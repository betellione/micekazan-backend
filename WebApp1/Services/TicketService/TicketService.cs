using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TicketService;

public class TicketService(ApplicationDbContext context, ITokenService tokenService, IQticketsApiProvider apiProvider) : ITicketService
{
    private readonly ILogger _logger = Log.ForContext<ITicketService>();

    public Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        // var ticket = await apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        return Task.FromResult((string?)string.Empty);
    }

    private async Task FillUpTicket(Ticket ticket, string clientEmail, long showId)
    {
        var clientId = await context.Clients.Where(x => x.Email == clientEmail).Select(x => x.Id).FirstOrDefaultAsync();
        if (clientId == default) throw new Exception($"Client with email {clientEmail} not found");

        var eventId = await context.Events.Where(x => x.ForeignShowIds.Contains(showId)).Select(x => x.Id).FirstOrDefaultAsync();
        if (eventId == default) throw new Exception($"Event with show with ID {showId} not found");

        ticket.ClientId = clientId;
        ticket.EventId = eventId;
    }

    public async Task<bool> ImportTickets(Guid userId)
    {
        var token = await tokenService.GetToken(userId);
        if (token is null) return false;

        var tickets = apiProvider.GetTickets(token);

        await foreach (var ticketForeign in tickets)
        {
            try
            {
                var ticket = new Ticket { Barcode = ticketForeign.Barcode, };
                await FillUpTicket(ticket, ticketForeign.ClientEmail, ticketForeign.ShowId);
                context.Tickets.Add(ticket);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving ticket {Ticket} in the DB", ticketForeign);
            }
        }

        return true;
    }
}