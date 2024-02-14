namespace WebApp1.Models;

public class CreatorToken
{
    public Guid CreatorId { get;set; }
    public string Token { get; set; } = null!;

    public User Creator { get; set; } = null!;
}