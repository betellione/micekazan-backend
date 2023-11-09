using WebApp1.Models;

namespace WebApp1.Services.TicketService;

public interface ITicketService
{
    public Task<string?> GetTicketPdfUri(string barcode);
    public Task<Ticket?> CreateTicket(string barcode);
}