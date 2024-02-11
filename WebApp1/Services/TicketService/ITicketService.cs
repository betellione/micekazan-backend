namespace WebApp1.Services.TicketService;

public interface ITicketService
{
    Task<Stream?> GetTicketPdf(Guid scannerId, string barcode);
    Task<bool> ImportTickets(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> SetPassTime(string barcode);
}