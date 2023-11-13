
namespace WebApp1.Models;

public class Client
{
    public long Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Surname { get; set; } = null!;
    public string? Patronymic { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? OrganizationName { get; set; } = null!;
    public string? WorkPosition { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}