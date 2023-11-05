using System.Net.Http.Headers;
using System.Text;
using WebApp1.External.Qtickets.Contracts.Responses;

namespace WebApp1.External.Qtickets;

public class QticketsApiProvider(IHttpClientFactory httpClientFactory) : IQticketsApiProvider
{
    public async Task<IEnumerable<Event>> GetActiveEvents(string token)
    {
        var httpClient = httpClientFactory.CreateClient("Qtickets");
        var result = new List<Event>();

        for (var page = 1; page <= 10; ++page)
        {
            // language=json
            var query = $$"""
                          {
                            "select": ["id", "name", "city.name", "shows.start_date", "shows.finish_date"],
                            "where": [
                              {
                                "column": "is_active",
                                "value": 1
                              }
                            ],
                            "perPage": 100,
                            "page": {{page}}
                          }
                          """;

            using var content = new StringContent(query, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(httpClient.BaseAddress!, "events");
            request.Content = content;
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

            using var response = await httpClient.SendAsync(request);

            var eventsBase = await response.Content.ReadFromJsonAsync<BaseResponse<Event>>();
            var total = eventsBase?.Paging?.Total ?? 0;
            var events = eventsBase?.Data.ToList();

            if (events is null || events.Count == 0) break;

            foreach (var @event in events)
            foreach (var show in @event.Shows)
            {
                show.StartDate = show.StartDate.ToUniversalTime();
                show.FinishDate = show.FinishDate.ToUniversalTime();
            }

            result.AddRange(events);

            if (events.Count < 100 || 100 * (page - 1) + events.Count >= total) break;
        }

        return result;
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