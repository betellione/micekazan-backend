using WebApp1.External.Qtickets.Contracts.Responses;

namespace WebApp1.External.Qtickets;

public interface IQticketsApiProvider
{
    public Task<IEnumerable<Event>> GetActiveEvents(string token);
    public Task<Ticket?> GetTicket(string barcode, string token);
}