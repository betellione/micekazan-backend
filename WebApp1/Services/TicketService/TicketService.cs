using WebApp1.External.Qtickets;
using WebApp1.Models;

namespace WebApp1.Services.TicketService;

public class TicketService(IQticketsApiProvider apiProvider) : ITicketService
{
    public async Task<string?> GetTicketPdfUri(string barcode)
    {
        // TODO: Get token.
        var ticket = await apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        return ticket?.PdfUri;
    }

    public async Task<Ticket?> CreateTicket(string barcode)
    {
        // TODO: Get token.
        var apiTicket = await apiProvider.GetTicket(barcode, "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c");
        if (apiTicket is null) return null;

        var ticket = new Ticket
        {
            Name = apiTicket.Name,
            Surname = apiTicket.Surname,
            Patronymic = apiTicket.Patronymic,
            PassedAt = DateTime.UtcNow
        };
        return ticket;
    }
}