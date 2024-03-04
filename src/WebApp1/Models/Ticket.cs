using WebApp1.Data.Batching;

namespace WebApp1.Models;

public class Ticket : IBatchable
{
    public long ForeignId { get; set; }
    public string Barcode { get; set; } = null!;
    public long EventId { get; set; }
    public long ClientId { get; set; }
    public DateTime? PassedAt { get; set; }

    public Event Event { get; set; } = null!;
    public Client Client { get; set; } = null!;
}