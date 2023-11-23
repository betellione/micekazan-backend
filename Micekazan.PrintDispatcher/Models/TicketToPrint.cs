namespace Micekazan.PrintDispatcher.Models;

public class TicketToPrint
{
    public long Id { get; set; }
    public string Barcode { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string PrintingToken { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
}