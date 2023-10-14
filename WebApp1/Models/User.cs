using Microsoft.AspNetCore.Identity;

namespace WebApp1.Models;

public class User : IdentityUser<Guid>
{
    public string? Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public ICollection<Event> Event { get; set; } = new List<Event>();
}