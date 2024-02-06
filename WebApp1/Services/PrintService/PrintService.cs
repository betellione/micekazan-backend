using Serilog;
using WebApp1.Data.Stores;
using WebApp1.Services.TicketService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.PrintService;

public class PrintService : IPrintService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger = Log.ForContext<IPrintService>();
    private readonly IScannerStore _scannerStore;
    private readonly ITicketService _ticketService;

    public PrintService(IHttpClientFactory httpClientFactory, IScannerStore scannerStore, ITicketService ticketService)
    {
        _httpClientFactory = httpClientFactory;
        _scannerStore = scannerStore;
        _ticketService = ticketService;
    }

    public async Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken, string barcode)
    {
        var httpClient = _httpClientFactory.CreateClient("PrintService");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(ticket);
        using var printingTokenContent = new StringContent(printingToken);
        using var barcodeContent = new StringContent(barcode);
        
        content.Add(fileContent, "file", "ticket.pdf");
        content.Add(printingTokenContent, "printingToken");
        content.Add(barcodeContent, "barcode");

        try
        {
            using var response = await httpClient.PostAsync("enqueue", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Cannot add ticket to PDF queue with token {PrintingToken}", printingToken);
            return false;
        }
    }
    
    public async Task<bool> PrintTicket(string code, Guid userId)
    {
        var printingToken = await _scannerStore.GetScannerPrintingToken(userId);
        if (printingToken is null) return false;

        await using var pdf = await _ticketService.GetTicketPdf(userId, code);
        if (pdf is null) return false;

        await AddTicketToPrintQueue(pdf, printingToken, code);

        return true;
    }
}