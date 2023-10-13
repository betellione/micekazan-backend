namespace Micekazan.Api.Models;

public class Event
{
    public long EventId { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime StartedAt { get; set; }
    public DateTime FinishesAt { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}