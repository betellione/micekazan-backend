namespace Micekazan.Api.Models;

public class Ticket
{
    public long Id { get; set; }
    public long ForeignId { get; set; }
    public string Barcode { get; set; } = null!;
    public string SeatName { get; set; } = null!;
    public decimal Price { get; set; }
    
    public long EventId { get; set; }
    public Event Event { get; set; } = null!;
}