using Microsoft.AspNetCore.Identity;

namespace WebApp1.Models;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public ICollection<CreatorToken> Tokens { get; set; } = new List<CreatorToken>();
    public ICollection<TokenUpdate> TokenUpdates { get; set; } = new List<TokenUpdate>();
    public ICollection<Event> EventsCreated { get; set; } = new List<Event>();
    public ICollection<EventCollector> EventsToCollect { get; set; } = new List<EventCollector>();
}