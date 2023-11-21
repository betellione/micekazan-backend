using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TicketService;

public class TicketService : ITicketService
{
    private readonly ILogger _logger = Log.ForContext<ITicketService>();
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IQticketsApiProvider _apiProvider;

    public TicketService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
        IQticketsApiProvider apiProvider)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _apiProvider = apiProvider;
    }

    public async Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        // var ticket = await _apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ticket2 = await context.Tickets.FirstOrDefaultAsync(x => x.Barcode == barcode);
        if (ticket2 != null) ticket2.PassedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return (string?)string.Empty;
    }

    /*public async Task<Stream> GetTicketPdf(string barcode)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Barcode == barcode);

        if (ticket is null) return Task.FromException();
        if (ticket.Barcode is null)
        {

        }
    }*/

    public async Task<Ticket?> SetPassTimeOrFalse(string barcode)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Barcode == barcode);
        return ticket;
    }

    public async Task<bool> ImportTickets(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var token = await _tokenService.GetToken(userId);
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