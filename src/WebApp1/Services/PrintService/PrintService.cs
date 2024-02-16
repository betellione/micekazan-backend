using Serilog;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.PrintService;

public class PrintService : IPrintService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger = Log.ForContext<IPrintService>();

    public PrintService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> AddTicketToPrintQueue(Stream ticket, string printingToken, string barcode)
    {
        var httpClient = _httpClientFactory.CreateClient("PrintService");

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(ticket);
        using var printingTokenContent = new StringContent(printingToken);
        using var barcodeContent = new StringContent(barcode);

        content.Add(barcodeContent, "barcode");
        content.Add(printingTokenContent, "printingToken");
        content.Add(fileContent, "file", "ticket.pdf");

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
}