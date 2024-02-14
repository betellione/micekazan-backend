using WebApp1.External.Qtickets.Contracts.Responses;

namespace WebApp1.External.Qtickets;

public interface IQticketsApiProvider
{
    IAsyncEnumerable<Event> GetEvents(string token, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Client> GetClients(string token, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Ticket> GetTickets(string token, CancellationToken cancellationToken = default);
    Task<ClientData?> GetTicketClientData(string barcode, string token, CancellationToken cancellationToken = default);
}