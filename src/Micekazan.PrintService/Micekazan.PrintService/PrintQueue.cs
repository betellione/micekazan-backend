using System.Threading.Channels;
using Micekazan.PrintDispatcher.Domain.Contracts;
using Micekazan.PrintService.PrintProvider;

namespace Micekazan.PrintService;

public class PrintQueue : BackgroundService
{
    private static readonly PrintSettings PrintSettings = new()
    {
        PaperWidth = 297,
        PaperHeight = 210,
        Margins = [10, 10, 10, 10,],
        StartPoint = (10, 10),
    };

    private readonly Channel<Document> _ch;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PrintQueue> _logger;
    private readonly IPrintProvider _printProvider;

    public PrintQueue(IHttpClientFactory httpClientFactory, IPrintProvider printProvider, Channel<Document> ch,
        ILogger<PrintQueue> logger)
    {
        _httpClientFactory = httpClientFactory;
        _printProvider = printProvider;
        _ch = ch;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await _ch.Reader.WaitToReadAsync(stoppingToken))
            {
                try
                {
                    var doc = await _ch.Reader.ReadAsync(stoppingToken);
                    var httpClient = _httpClientFactory.CreateClient("PrintApi");
                    using var response = await httpClient.GetAsync(doc.DocumentUri, stoppingToken);

                    await using var stream = await response.Content.ReadAsStreamAsync(stoppingToken);

                    await _printProvider.PrintDocument(stream, PrintSettings);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: {ErrorMessage}", e.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {ErrorMessage}", e.Message);
        }
    }
}