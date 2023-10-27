namespace WebApp1.Models;

public class Ticket
{
    public long Id { get; set; }
    public string Barcode { get; set; } = null!;
    public long EventId { get; set; }

    public Event Event { get; set; } = null!;
}