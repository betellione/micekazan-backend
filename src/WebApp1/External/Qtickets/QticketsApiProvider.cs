using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using WebApp1.External.Qtickets.Contracts.Responses;
using ILogger = Serilog.ILogger;

namespace WebApp1.External.Qtickets;

public class QticketsApiProvider : IQticketsApiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger = Log.ForContext<IQticketsApiProvider>();

    public QticketsApiProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IAsyncEnumerable<Event> GetEvents(string token, CancellationToken cancellationToken = default)
    {
        var builder = new QueryBuilder().Select("id", "name", "city.name", "shows.id", "shows.start_date", "shows.finish_date");
        return GetDataList<Event>("events", token, builder, action: Func, cancellationToken: cancellationToken);

        static void Func(Event @event)
        {
            @event.ShowIds = new List<long>();

            foreach (var show in @event.Shows)
            {
                ((List<long>)@event.ShowIds).Add(show.Id);
                show.StartDate = show.StartDate.ToUniversalTime();
                show.FinishDate = show.FinishDate.ToUniversalTime();
            }
        }
    }

    public IAsyncEnumerable<Client> GetClients(string token, CancellationToken cancellationToken = default)
    {
        var builder = new QueryBuilder()
            .Select("id", "email", "details.name", "details.middlename", "details.surname", "details.phone")
            .OrderByAscending("id");
        return GetDataList<Client>("clients", token, builder, 50, null, cancellationToken);
    }

    public IAsyncEnumerable<Ticket> GetTickets(string token, CancellationToken cancellationToken = default)
    {
        var builder = new QueryBuilder().Select("id", "barcode", "client_email", "show_id").Where("barcode", null, "not null");
        return GetDataList<Ticket>("baskets", token, builder, 50, null, cancellationToken);
    }

    public async Task<ClientData?> GetTicketClientData(string barcode, string token, CancellationToken cancellationToken = default)
    {
        var builder = new QueryBuilder()
            .Select("client_email", "client_phone", "client_name", "client_surname", "client_middlename", "organization_name", "work_position")
            .Where("barcode", barcode);

        return await GetDataList<ClientData>("baskets", token, builder, 1, null, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async IAsyncEnumerable<T> GetDataList<T>(string apiMethod, string token, QueryBuilder builder, int maxPages = 10,
        Action<T>? action = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int itemsPerPage = 100;

        var httpClient = _httpClientFactory.CreateClient("Qtickets");

        for (var page = 1; page <= maxPages; ++page)
        {
            BaseResponse<T>? baseDataList;

            try
            {
                var query = builder.PerPage(itemsPerPage).Page(page).Build();
                using var content = new StringContent(query, Encoding.UTF8, "application/json");
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(httpClient.BaseAddress!, apiMethod);
                request.Content = content;
                request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

                using var response = await httpClient.SendAsync(request, cancellationToken);
                baseDataList = await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
                if (baseDataList is null) yield break;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while getting data list from Qtickets API with method {ApiMethod}", apiMethod);
                yield break;
            }

            var total = baseDataList.Paging?.Total ?? 0;
            var count = 0;

            foreach (var item in baseDataList.Data)
            {
                action?.Invoke(item);
                count += 1;
                yield return item;
            }

            if (count < itemsPerPage || itemsPerPage * (page - 1) + count >= total) yield break;
        }
    }
}