namespace WebApp1.Models;

public class EventScanner
{
    public Guid ScannerId { get; set; }
    public long EventId { get; set; }
    public long? TicketPdfTemplateId { get; set; }
    public string? PrintingToken { get; set; }

    public User Scanner { get; set; } = null!;
    public Event Event { get; set; } = null!;
    public TicketPdfTemplate? TicketPdfTemplate { get; set; }
}