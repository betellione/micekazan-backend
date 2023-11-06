namespace WebApp1.ViewModels.Event;

public class EventDetails
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorUsername { get; set; } = null!;
    public IEnumerable<Scanner> Scanners { get; set; } = Enumerable.Empty<Scanner>();
}