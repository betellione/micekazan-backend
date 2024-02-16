using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.ClientService;
using WebApp1.Services.PdfGenerator;
using WebApp1.Services.QrCodeGenerator;
using WebApp1.Services.TemplateService;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    private readonly IQticketsApiProvider _apiProvider;
    private readonly IClientService _clientService;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<ITicketService>();
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IServiceProvider _sp;
    private readonly ITemplateService _templateService;
    private readonly ITokenService _tokenService;

    public TicketService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
        IQticketsApiProvider apiProvider, IClientService clientService, IServiceProvider sp, IPdfGenerator pdfGenerator,
        IQrCodeGenerator qrCodeGenerator, ITemplateService templateService)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _apiProvider = apiProvider;
        _clientService = clientService;
        _pdfGenerator = pdfGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _templateService = templateService;
        _sp = sp;
    }

    public async Task<Stream?> GetTicketPdf(Guid scannerId, string barcode)
    {
        var info = await _clientService.AddClientData(barcode);
        if (info is null) return null;

        await using var context = await _contextFactory.CreateDbContextAsync();
        var template = await _templateService.GetTemplateForScanner(scannerId);
        await using var model = template is null ? CreateDefaultDocumentModel(info) : CreateDocumentModelFromTemplate(info, template);

        var pdf = _pdfGenerator.GenerateTicketPdf(model);

        return pdf;
    }

    public async Task<bool> ImportTickets(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var token = await _tokenService.GetCurrentOrganizerToken(userId);
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

    private Stream GetQrCode(string token)
    {
        var linkGenerator = _sp.GetRequiredService<LinkGenerator>();
        var accessor = _sp.GetRequiredService<IHttpContextAccessor>();

        var qrContent = linkGenerator.GetUriByAction(accessor.HttpContext!, "InfoToShow", "Client", new { token, }) ?? string.Empty;
        var qr = _qrCodeGenerator.GenerateQrCode(qrContent);

        return qr;
    }

    private TicketDocumentModel CreateDefaultDocumentModel(InfoToShow info)
    {
        return new TicketDocumentModel
        {
            Name = info.ClientName,
            Surname = info.ClientSurname,
            QrStream = GetQrCode(info.Token),
            FontColor = "#000000",
            IsHorizontal = true,
            BackgroundPath = null,
            LogoPath = null,
        };
    }

    private TicketDocumentModel CreateDocumentModelFromTemplate(InfoToShow info, TicketPdfTemplate template)
    {
        return new TicketDocumentModel
        {
            Name = template.HasName ? info.ClientName : null,
            Surname = template.HasSurname ? info.ClientSurname : null,
            QrStream = template.HasQrCode ? GetQrCode(info.Token) : null,
            FontColor = template.TextColor,
            IsHorizontal = template.IsHorizontal,
            BackgroundPath = template.BackgroundUri,
            LogoPath = template.LogoUri,
        };
    }
}