using WebApp1.Models;

namespace WebApp1.Services.TicketService;

public interface ITicketService
{
    Task<Stream?> CreateTicketPdf(Guid scannerId, string qrCodeData, InfoToShow info);
    Task<bool> ImportTickets(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> SetPassTime(string barcode);
}