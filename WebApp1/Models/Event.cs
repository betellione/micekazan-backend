namespace WebApp1.Models;

public class Event
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public Guid CreatorId { get; set; }

    public User Creator { get; set; } = null!;
    public ICollection<EventCollector> Collectors { get; set; } = new List<EventCollector>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}