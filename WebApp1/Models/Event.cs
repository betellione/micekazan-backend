namespace WebApp1.Models;

public class Event
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public long[] ForeignShowIds { get; set; } = Array.Empty<long>();
    public Guid CreatorId { get; set; }

    public User Creator { get; set; } = null!;
    public IEnumerable<EventScanner> Collectors => _collectors ??= new List<EventScanner>();
    public IEnumerable<Ticket> Tickets => _tickets ??= new List<Ticket>();
    public IEnumerable<Screen> Screens => _screens ??= new List<Screen>();

    private List<EventScanner>? _collectors;
    private List<Ticket>? _tickets;
    private List<Screen>? _screens;
}