using Microsoft.AspNetCore.Identity;
using WebApp1.Enums;

namespace WebApp1.Models;

public class User : IdentityUser<Guid>
{
    public string? Name { get; set; } = null!;
    public string? Surname { get; set; } = null!;
    public string? Patronymic { get; set; } = null!;
    public string? City { get; set; } = null!;
    public ActivityTypes? Activity { get; set; } = ActivityTypes.Individual; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public ICollection<CreatorToken> Tokens { get; set; } = new List<CreatorToken>();
    public ICollection<TokenUpdate> TokenUpdates { get; set; } = new List<TokenUpdate>();
    public ICollection<Event> EventsCreated { get; set; } = new List<Event>();
    public ICollection<TicketPdfTemplate> TicketPdfTemplates { get; set; } = new List<TicketPdfTemplate>();
    public ICollection<EventCollector> EventsToCollect { get; set; } = new List<EventCollector>();
}