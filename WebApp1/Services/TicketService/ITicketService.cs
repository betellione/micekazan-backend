namespace WebApp1.Services.TicketService;

public interface ITicketService
{
    public Task<string?> GetTicketPdfUri(string barcode);
}