namespace WebApp1.Models;

public class EventCollector
{
    public Guid CollectorId { get; set; }
    public long EventId { get; set; }

    public User Collector { get; set; } = null!;
    public Event Event { get; set; } = null!;
}