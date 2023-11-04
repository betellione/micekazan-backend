using WebApp1.Models;
using Event = WebApp1.External.Qtickets.Contracts.Responses.Event;

namespace WebApp1.External.Qtickets;

public interface IQticketsApiProvider
{
    public Task<IEnumerable<Event>> GetActiveEvents(string token);
    public Task<Ticket?> GetTicket(string barcode, string token);
}