namespace WebApp1.Models;

public class Ticket
{
    public long Id { get; set; }
    public string Barcode { get; set; } = null!;
    public long EventId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public DateTime PassedAt { get; set; } = DateTime.UtcNow;

    public Event Event { get; set; } = null!;
}