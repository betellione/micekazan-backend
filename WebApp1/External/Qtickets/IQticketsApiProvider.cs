using WebApp1.External.Qtickets.Contracts.Responses;

namespace WebApp1.External.Qtickets;

public interface IQticketsApiProvider
{
    public IAsyncEnumerable<Event> GetEvents(string token);
    public IAsyncEnumerable<Client> GetClients(string token);
    public IAsyncEnumerable<Ticket> GetTickets(string token);
    public Task<Ticket?> GetTicket(string barcode, string token);
}