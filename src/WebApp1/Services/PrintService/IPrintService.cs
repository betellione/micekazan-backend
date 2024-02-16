namespace WebApp1.Services.PrintService;

public interface IPrintService
{
    Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken, string barcode);
}