using System.Threading.Channels;
using Micekazan.PrintDispatcher.Contracts;
using Micekazan.PrintService.PrintProvider;

namespace Micekazan.PrintService;

public class PrintQueue : BackgroundService
{
    private static readonly PrintSettings PrintSettings = new()
    {
        PaperWidth = 297,
        PaperHeight = 210,
        Margins = new[] { 10, 10, 10, 10, },
        StartPoint = (10, 10),
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPrintProvider _printProvider;
    private readonly Channel<Document> _ch;

    public PrintQueue(IHttpClientFactory httpClientFactory, IPrintProvider printProvider, Channel<Document> ch)
    {
        _httpClientFactory = httpClientFactory;
        _printProvider = printProvider;
        _ch = ch;
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
                    var httpClient = _httpClientFactory.CreateClient("Qtickets");
                    using var response = await httpClient.GetAsync(doc.DocumentUri, stoppingToken);

                    await using var stream = await response.Content.ReadAsStreamAsync(stoppingToken);

                    await _printProvider.PrintDocument(stream, PrintSettings);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception)
        {
            // ignored
        }
    }
}