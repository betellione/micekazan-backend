using Microsoft.AspNetCore.Identity;
using WebApp1.Enums;

namespace WebApp1.Models;

public class User : IdentityUser<Guid>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string? City { get; set; }
    public ActivityTypes? Activity { get; set; } = ActivityTypes.Individual;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public IEnumerable<CreatorToken> Tokens => _tokens ??= new List<CreatorToken>();
    public IEnumerable<TokenUpdate> TokenUpdates => _tokenUpdates ??= new List<TokenUpdate>();
    public IEnumerable<Event> EventsCreated => _eventsCreated ??= new List<Event>();
    public IEnumerable<TicketPdfTemplate> TicketPdfTemplates => _ticketPdfTemplates ??= new List<TicketPdfTemplate>();
    public IEnumerable<EventScanner> EventsToCollect => _eventsToCollect ??= new List<EventScanner>();
    public IEnumerable<UserConfirmationPhoneCall> ConfirmationPhoneCalls => _confirmationPhoneCalls ??= new List<UserConfirmationPhoneCall>();

    private List<CreatorToken>? _tokens;
    private List<TokenUpdate>? _tokenUpdates;
    private List<Event>? _eventsCreated;
    private List<TicketPdfTemplate>? _ticketPdfTemplates;
    private List<EventScanner>? _eventsToCollect;
    private List<UserConfirmationPhoneCall>? _confirmationPhoneCalls;
}