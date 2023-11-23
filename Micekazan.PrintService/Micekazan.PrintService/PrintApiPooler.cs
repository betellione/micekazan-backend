using System.Threading.Channels;
using Micekazan.PrintDispatcher.Domain.Contracts;

namespace Micekazan.PrintService;

public class PrintApiPooler : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Channel<Document> _ch;

    public PrintApiPooler(IHttpClientFactory httpClientFactory, Channel<Document> ch)
    {
        _httpClientFactory = httpClientFactory;
        _ch = ch;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                    var httpClient = _httpClientFactory.CreateClient("PrintApi");
                    using var response = await httpClient.GetAsync("update", stoppingToken);
                    var update = await response.Content.ReadFromJsonAsync<Update>(stoppingToken);

                    switch (update!.Kind)
                    {
                        case UpdateKind.PrintCommand:
                            await _ch.Writer.WriteAsync(update.Document!, stoppingToken);
                            break;
                        case UpdateKind.NoContent:
                            continue;
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