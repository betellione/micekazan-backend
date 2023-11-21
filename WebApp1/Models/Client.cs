namespace WebApp1.Models;

public class Client
{
    public long Id { get; set; }
    public long ForeignId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public IEnumerable<Ticket> Tickets => _tickets ??= new List<Ticket>();

    private List<Ticket>? _tickets;
}