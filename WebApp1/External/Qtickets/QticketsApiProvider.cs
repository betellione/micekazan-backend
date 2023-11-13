using System.Net.Http.Headers;
using System.Text;
using Serilog;
using WebApp1.External.Qtickets.Contracts.Responses;
using ILogger = Serilog.ILogger;

namespace WebApp1.External.Qtickets;

public class QticketsApiProvider(IHttpClientFactory httpClientFactory) : IQticketsApiProvider
{
    private readonly ILogger _logger = Log.ForContext<IQticketsApiProvider>();

    public async IAsyncEnumerable<Event> GetEvents(string token)
    {
        const int itemsPerPage = 100;
        const int pages = 10;

        var httpClient = httpClientFactory.CreateClient("Qtickets");

        for (var page = 1; page <= pages; ++page)
        {
            // language=json
            var query = $$"""
                          {
                            "select": ["id", "name", "city.name", "shows.start_date", "shows.finish_date"],
                            "perPage": {{itemsPerPage}},
                            "page": {{page}}
                          }
                          """;

            BaseResponse<Event>? eventsBase;

            try
            {
                using var content = new StringContent(query, Encoding.UTF8, "application/json");
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(httpClient.BaseAddress!, "events");
                request.Content = content;
                request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

                using var response = await httpClient.SendAsync(request);
                eventsBase = await response.Content.ReadFromJsonAsync<BaseResponse<Event>>();
                if (eventsBase is null) yield break;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while getting events from Qtickets API");
                yield break;
            }

            var total = eventsBase.Paging?.Total ?? 0;
            var count = 0;

            foreach (var @event in eventsBase.Data)
            {
                foreach (var show in @event.Shows)
                {
                    show.StartDate = show.StartDate.ToUniversalTime();
                    show.FinishDate = show.FinishDate.ToUniversalTime();
                }

                count += 1;
                yield return @event;
            }

            if (count < itemsPerPage || itemsPerPage * (page - 1) + count >= total) yield break;
        }
    }

    public async Task<Ticket?> GetTicket(string barcode, string token)
    {
        var httpClient = httpClientFactory.CreateClient("Qtickets");

        // language=json
        var query = $$"""
                      {
                        "where": [
                          {
                            "column": "barcode",
                            "value": "{{barcode}}"
                          }
                        ],
                        "perPage": 100
                      }
                      """;

        using var content = new StringContent(query, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri(httpClient.BaseAddress!, "baskets");
        request.Content = content;
        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

        using var response = await httpClient.SendAsync(request);

        var ticketBase = await response.Content.ReadFromJsonAsync<BaseResponse<Ticket>>();
        var ticket = ticketBase?.Data.FirstOrDefault();

        return ticket;
    }
}