namespace WebApp1.Models;

public class TokenUpdate
{
    public long Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Creator { get; set; } = null!;
}