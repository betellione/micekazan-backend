
namespace WebApp1.Models;

public class Client
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}