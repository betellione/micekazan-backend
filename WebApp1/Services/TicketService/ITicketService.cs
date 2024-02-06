namespace WebApp1.Services.TicketService;

public interface ITicketService
{
    public Task<Stream?> GetTicketPdf(Guid scannerId, string barcode);
    public Task<bool> ImportTickets(Guid userId);
    public Task<bool> SetPassTimeOrFalse(string barcode);
}