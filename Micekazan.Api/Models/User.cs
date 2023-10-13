using Micekazan.Api.Types;

namespace Micekazan.Api.Models;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    
    public RoleType RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public ICollection<Event> Event { get; set; } = new List<Event>();
}