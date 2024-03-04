using System.Runtime.CompilerServices;
using Micekazan.Infrastructure.MediaGenerators;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.Data.Batching;
using WebApp1.Data.Stores;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.PdfGenerator;
using WebApp1.Services.TemplateService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    private readonly IQticketsApiProvider _apiProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<ITicketService>();
    private readonly IPdfGenerator _pdfGenerator;
    private readonly ITemplateService _templateService;
    private readonly ITokenStore _tokenStore;

    public TicketService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenStore tokenStore,
        IQticketsApiProvider apiProvider, IPdfGenerator pdfGenerator, ITemplateService templateService)
    {
        _contextFactory = contextFactory;
        _tokenStore = tokenStore;
        _apiProvider = apiProvider;
        _pdfGenerator = pdfGenerator;
        _templateService = templateService;
    }

    public async Task<bool> ImportTickets(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var token = await _tokenStore.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var tickets = _apiProvider.GetTickets(token, cancellationToken).Select(x => new Ticket
        {
            ForeignId = x.Id,
            Barcode = x.Barcode,
            EventId = x.EventId!.Value,
            ClientId = x.ClientId!.Value,
        });

        var batchWorker = new BatchWorker<Ticket>(_contextFactory, UpdateAction, 100);
        await batchWorker.Batch(tickets, cancellationToken);

        return true;

        static void UpdateAction(Ticket oldTicket, Ticket newTicket)
        {
            oldTicket.Barcode = newTicket.Barcode;
            oldTicket.ClientId = newTicket.ClientId;
            oldTicket.EventId = newTicket.EventId;
        }
    }

    public async Task<Stream?> CreateTicketPdf(Guid scannerId, string qrCodeData, InfoToShow info)
    {
        var printingToken = await _tokenStore.GetScannerPrintingToken(scannerId);
        if (printingToken is null) return null;

        var template = await _templateService.GetTemplateForScanner(scannerId);
        await using var model = CreateDocumentModel(info, qrCodeData, template);
        var document = _pdfGenerator.GeneratePdfDocument(model);

        return document;
    }

    public async Task<bool> SetPassTime(string barcode)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Barcode == barcode);
        if (ticket is null || ticket.PassedAt is not null) return false;

        ticket.PassedAt = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while setting pass time for ticket with barcode {Barcode} in the DB", barcode);
            return false;
        }
    }

    private static TicketDocumentModel CreateDefaultDocumentModel(InfoToShow info, string qrData)
    {
        return new TicketDocumentModel
        {
            Name = info.ClientName,
            Surname = info.ClientSurname,
            QrStream = QrCodeGenerator.GenerateQrCode(qrData),
            FontColor = "#000000",
            IsHorizontal = true,
            BackgroundPath = null,
            LogoPath = null,
        };
    }

    private static TicketDocumentModel CreateDocumentModel(InfoToShow info, string qrData, TicketPdfTemplate? template = null)
    {
        if (template is null) return CreateDefaultDocumentModel(info, qrData);

        return new TicketDocumentModel
        {
            Name = template.HasName ? info.ClientName : null,
            Surname = template.HasSurname ? info.ClientSurname : null,
            QrStream = template.HasQrCode ? QrCodeGenerator.GenerateQrCode(qrData) : null,
            FontColor = template.TextColor,
            IsHorizontal = template.IsHorizontal,
            BackgroundPath = template.BackgroundUri,
            LogoPath = template.LogoUri,
        };
    }
}

file class BatchWorker<T> where T : class, IBatchable
{
    private readonly int _batchSize;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<BatchWorker<T>>();
    private readonly List<T> _toAdd;
    private readonly Dictionary<long, T> _toUpdate;
    private readonly Action<T, T> _updateAction;

    public BatchWorker(IDbContextFactory<ApplicationDbContext> contextFactory, Action<T, T> updateAction, int batchSize)
    {
        _contextFactory = contextFactory;
        _batchSize = batchSize;
        _updateAction = updateAction;
        _toAdd = new List<T>(batchSize);
        _toUpdate = new Dictionary<long, T>(batchSize);
    }

    public async Task Batch(IAsyncEnumerable<T> newEntities, CancellationToken cancellationToken = default)
    {
        await using var oldEnumerator = RetrieveOldIds(cancellationToken).GetAsyncEnumerator(cancellationToken);
        await using var newEnumerator = newEntities.GetAsyncEnumerator(cancellationToken);

        if (!await oldEnumerator.MoveNextAsync())
        {
            await ProcessRemainingEntities(newEnumerator, cancellationToken);
        }
        else
        {
            if (!await newEnumerator.MoveNextAsync()) return;
            while (true)
            {
                var oldForeignId = oldEnumerator.Current;
                var newEntity = newEnumerator.Current;

                if (oldForeignId == newEntity.ForeignId)
                {
                    await UpdateEntity(newEntity, cancellationToken);
                    if (!await oldEnumerator.MoveNextAsync())
                    {
                        await ProcessRemainingEntities(newEnumerator, cancellationToken);
                        break;
                    }

                    if (!await newEnumerator.MoveNextAsync()) break;
                }
                else if (oldForeignId > newEntity.ForeignId)
                {
                    await AddEntity(newEntity, cancellationToken);
                    if (!await newEnumerator.MoveNextAsync()) break;
                }
                else
                {
                    if (!await oldEnumerator.MoveNextAsync())
                    {
                        await AddEntity(newEntity, cancellationToken);
                        await ProcessRemainingEntities(newEnumerator, cancellationToken);
                        break;
                    }
                }
            }
        }

        await ForceSaveBatched(cancellationToken);
    }

    private async Task ProcessRemainingEntities(IAsyncEnumerator<T> enumerator, CancellationToken cancellationToken)
    {
        while (await enumerator.MoveNextAsync()) await AddEntity(enumerator.Current, cancellationToken);
    }

    private async ValueTask UpdateEntity(T newEntity, CancellationToken cancellationToken)
    {
        _toUpdate.TryAdd(newEntity.ForeignId, newEntity);
        if (_toUpdate.Count != _batchSize) return;
        await UpdateSave(cancellationToken);
    }

    private async ValueTask AddEntity(T entity, CancellationToken cancellationToken)
    {
        _toAdd.Add(entity);
        if (_toAdd.Count != _batchSize) return;
        await AddSave(cancellationToken);
    }

    private async Task ForceSaveBatched(CancellationToken cancellationToken)
    {
        await AddSave(cancellationToken);
        await UpdateSave(cancellationToken);
    }

    private async Task UpdateSave(CancellationToken cancellationToken)
    {
        if (_toUpdate.Count == 0) return;

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var foreignIds = _toUpdate.Keys.ToArray();
        var oldEntities = await context.Set<T>()
            .Where(x => foreignIds.Contains(x.ForeignId))
            .ToDictionaryAsync(x => x.ForeignId, x => x, cancellationToken);

        foreach (var (foreignId, newData) in _toUpdate)
        {
            if (oldEntities.TryGetValue(foreignId, out var oldData)) _updateAction(oldData, newData);
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, $"Error while updating entities in the DB Set of {nameof(T)}");
        }

        _toUpdate.Clear();
    }

    private async Task AddSave(CancellationToken cancellationToken)
    {
        if (_toAdd.Count == 0) return;

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            context.Set<T>().AddRange(_toAdd);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, $"Error while saving entities in the DB Set of {nameof(T)}");
        }

        _toAdd.Clear();
    }

    private async IAsyncEnumerable<long> RetrieveOldIds([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lastMaxForeignId = 0L;

        while (true)
        {
            var max = lastMaxForeignId;
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var ids = await context.Set<T>()
                .Where(x => x.ForeignId > max)
                .OrderBy(x => x.ForeignId)
                .Select(x => x.ForeignId)
                .Take(_batchSize)
                .ToListAsync(cancellationToken);

            if (ids.Count == 0) yield break;
            lastMaxForeignId = ids[^1];

            foreach (var id in ids) yield return id;
        }
    }
}