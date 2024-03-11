using Micekazan.Infrastructure.MediaGenerators;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
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

        var batchCounter = 0;
        var foreignTickets = _apiProvider.GetTickets(token, cancellationToken);
        var oldTicketBarcodes = (await context.Tickets.Select(x => x.Barcode).ToListAsync(cancellationToken)).ToHashSet();
        var clientEmails = await context.Clients.Select(x => new { x.Id, x.Email, }).ToListAsync(cancellationToken);
        var eventShowIds = await context.Events.Select(x => new { x.Id, x.ForeignShowIds, }).ToListAsync(cancellationToken);

        await foreach (var foreignTicket in foreignTickets)
        {
            try
            {
                var client = clientEmails.FirstOrDefault(x => x.Email == foreignTicket.ClientEmail);
                if (client is null) continue;
                var @event = eventShowIds.FirstOrDefault(x => x.ForeignShowIds.Contains(foreignTicket.ShowId));
                if (@event is null) continue;

                var ticket = new Ticket
                {
                    Barcode = foreignTicket.Barcode,
                    ClientId = client.Id,
                    EventId = @event.Id,
                    ForeignId = foreignTicket.Id,
                };

                if (oldTicketBarcodes.Contains(foreignTicket.Barcode))
                {
                    await UpdateTicket(ticket, context, cancellationToken);
                }
                else
                {
                    context.Tickets.Add(ticket);
                }

                if (++batchCounter < 100) continue;

                batchCounter = 0;
                await context.SaveChangesAsync(cancellationToken);
                context.ChangeTracker.Clear();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving tickets in the DB");
                context.ChangeTracker.Clear();
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

    private static async Task UpdateTicket(Ticket ticket, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var toUpdate = await context.Tickets
            .Where(x => x.Barcode == ticket.Barcode)
            .FirstOrDefaultAsync(cancellationToken);

        if (toUpdate is null) return;

        context.Tickets.Update(toUpdate);

        toUpdate.EventId = ticket.EventId;
        toUpdate.ClientId = ticket.ClientId;
        toUpdate.ForeignId = ticket.ForeignId;
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