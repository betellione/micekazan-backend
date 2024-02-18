using System.Threading.Channels;
using Micekazan.PrintDispatcher.Domain.Contracts;

namespace Micekazan.PrintService;

public class PrintApiPooler : BackgroundService
{
    private readonly Channel<Document> _ch;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PrintApiPooler> _logger;

    public PrintApiPooler(IHttpClientFactory httpClientFactory, Channel<Document> ch, ILogger<PrintApiPooler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _ch = ch;
        _logger = logger;
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
                    var update = await response.Content.ReadFromJsonAsync(
                        GlobalJsonSerializerContext.Default.Update, stoppingToken);

                    switch (update!.Kind)
                    {
                        case UpdateKind.PrintCommand:
                            await _ch.Writer.WriteAsync(update.Document!, stoppingToken);
                            break;
                        case UpdateKind.NoContent:
                            continue;
                        default:
                            throw new Exception("Unknown Update Kind");
                    }

                    var ack = new Acknowledgement { UpdateId = update.Id, };
                    using var ackResponse = await httpClient.PostAsJsonAsync(
                        "ack", ack, GlobalJsonSerializerContext.Default.Acknowledgement, stoppingToken);
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