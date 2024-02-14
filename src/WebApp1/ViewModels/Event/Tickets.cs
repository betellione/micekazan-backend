namespace WebApp1.ViewModels.Event;

public class Tickets
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public IEnumerable<PassedTickets> PassedTickets { get; set; } = Enumerable.Empty<PassedTickets>();
}