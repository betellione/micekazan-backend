using System.Threading.Channels;
using Micekazan.PrintDispatcher.Contracts;

namespace Micekazan.PrintService;

public class PrintApiPooler(IHttpClientFactory httpClientFactory, Channel<Document> chPrint) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var httpClient = httpClientFactory.CreateClient("PrintApi");
                    using var response = await httpClient.GetAsync("update", stoppingToken);
                    var update = await response.Content.ReadFromJsonAsync<Update>(stoppingToken);

                    switch (update!.Kind)
                    {
                        case UpdateKind.PrintCommand:
                            await chPrint.Writer.WriteAsync(update.Document!, stoppingToken);
                            break;
                        default:
                            throw new Exception();
                    }

                    using var ack = JsonContent.Create(new Acknowledgement { UpdateId = update.Id, });
                    using var ackResponse = await httpClient.PostAsync("ack", ack, stoppingToken);
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