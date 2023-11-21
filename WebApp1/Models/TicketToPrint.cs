namespace WebApp1.Models;

public class TicketToPrint
{
    public string Barcode { get; set; } = null!;
    public string Url { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}