namespace WebApp1.Services.PrintService;

public interface IPrintService
{
    public Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken, string barcode);
    public Task<bool> PrintTicket(string code, Guid userId);
}