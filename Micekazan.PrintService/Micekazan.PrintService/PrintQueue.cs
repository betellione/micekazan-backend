using System.Threading.Channels;
using Micekazan.PrintDispatcher.Contracts;
using Micekazan.PrintService.PrintProvider;

namespace Micekazan.PrintService;

public class PrintQueue(IHttpClientFactory httpClientFactory, IPrintProvider printProvider, Channel<Document> ch)
    : BackgroundService
{
    private static readonly PrintSettings PrintSettings = new()
    {
        PaperWidth = 297,
        PaperHeight = 210,
        Margins = new[] { 10, 10, 10, 10, },
        StartPoint = (10, 10),
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await ch.Reader.WaitToReadAsync(stoppingToken))
            {
                try
                {
                    var doc = await ch.Reader.ReadAsync(stoppingToken);
                    var httpClient = httpClientFactory.CreateClient("Qtickets");
                    using var response = await httpClient.GetAsync(doc.DocumentUri, stoppingToken);
                    
                    await using var stream = await response.Content.ReadAsStreamAsync(stoppingToken);

                    await printProvider.PrintDocument(stream, PrintSettings);
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