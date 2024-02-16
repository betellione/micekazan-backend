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

        var tickets = _apiProvider.GetTickets(token, cancellationToken);

        var batchCounter = 0;
        var existed = await context.Tickets.ToDictionaryAsync(x => x.Barcode, x => x.Id, cancellationToken);
        var clientEmailIdPairs = await context.Clients.ToDictionaryAsync(x => x.Email, x => x.Id, cancellationToken);
        var eventShowIdPairs = await context.Events.Select(x => new { x.Id, x.ForeignShowIds, }).ToListAsync(cancellationToken);

        await foreach (var ticketForeign in tickets)
        {
            try
            {
                if (!clientEmailIdPairs.TryGetValue(ticketForeign.ClientEmail, out var clientId))
                {
                    throw new Exception($"Client with email {ticketForeign.ClientEmail} not found");
                }

                var @event = eventShowIdPairs.FirstOrDefault(x => x.ForeignShowIds.Contains(ticketForeign.ShowId));
                if (@event is null) throw new Exception($"Event with show with ID {ticketForeign.ShowId} not found");

                var ticket = new Ticket { Barcode = ticketForeign.Barcode, ClientId = clientId, EventId = @event.Id, };

                if (existed.TryGetValue(ticketForeign.Barcode, out var ticketId))
                {
                    ticket.Id = ticketId;
                    context.Tickets.Update(ticket);
                }
                else
                {
                    context.Tickets.Add(ticket);
                }

                if (++batchCounter < 100) continue;
                batchCounter = 0;

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving tickets in the DB");
            }
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while saving tickets in the DB");
        }

        return true;
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