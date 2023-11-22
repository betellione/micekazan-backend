using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.ClientService;
using WebApp1.Services.PdfGenerator;
using WebApp1.Services.QrCodeGenerator;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    private readonly ILogger _logger = Log.ForContext<ITicketService>();
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IQticketsApiProvider _apiProvider;
    private readonly IClientService _clientService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IQrCodeGenerator _qrCodeGenerator;
    private readonly IServiceProvider _sp;

    public TicketService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
        IQticketsApiProvider apiProvider, IClientService clientService, IServiceProvider sp, IPdfGenerator pdfGenerator,
        IQrCodeGenerator qrCodeGenerator)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _apiProvider = apiProvider;
        _clientService = clientService;
        _pdfGenerator = pdfGenerator;
        _qrCodeGenerator = qrCodeGenerator;
        _sp = sp;
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

    public async Task<Stream?> GetTicketPdf(Guid scannerId, string barcode)
    {
        var info = await _clientService.AddClientData(barcode);
        if (info is null) return null;

        await using var context = await _contextFactory.CreateDbContextAsync();

        var template = await context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .Select(x => x.TicketPdfTemplate)
            .FirstOrDefaultAsync();

        await using var model = template is null ? CreateDefaultDocumentModel(info) : CreateDocumentModelFromTemplate(info, template);

        var pdf = _pdfGenerator.GenerateTicketPdf(model);

        return pdf;
    }

    public async Task<Ticket?> SetPassTimeOrFalse(string barcode)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Barcode == barcode);
        return ticket;
    }

    public async Task<bool> ImportTickets(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var token = await _tokenService.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var tickets = _apiProvider.GetTickets(token);

        var batchCounter = 0;
        var existed = (await context.Tickets.Select(x => x.Barcode).ToListAsync()).ToHashSet();
        var clientEmailIdPairs = await context.Clients.ToDictionaryAsync(x => x.Email, x => x.Id);
        var eventShowIdPairs = await context.Events.Select(x => new { x.Id, x.ForeignShowIds, }).ToListAsync();

        await foreach (var ticketForeign in tickets)
        {
            try
            {
                if (!clientEmailIdPairs.TryGetValue(ticketForeign.ClientEmail, out var clientId))
                {
                    throw new Exception($"Client with email {ticketForeign.ClientEmail} not found");
                }

                var eventId = eventShowIdPairs.FirstOrDefault(x => x.ForeignShowIds.Contains(ticketForeign.ShowId));
                if (eventId is null) throw new Exception($"Event with show with ID {ticketForeign.ShowId} not found");

                var ticket = new Ticket { Barcode = ticketForeign.Barcode, ClientId = clientId, EventId = eventId.Id, };

                if (existed.Contains(ticketForeign.Barcode))
                {
                    context.Tickets.Update(ticket);
                }
                else
                {
                    context.Tickets.Add(ticket);
                }

                if (++batchCounter < 100) continue;
                batchCounter = 0;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving tickets in the DB");
            }
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while saving tickets in the DB");
        }

        return true;
    }
}