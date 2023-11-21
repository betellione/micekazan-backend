namespace WebApp1.Models;

public class InfoToShow
{
    public long Id { get; set; }
    public string? Email { get; set; } = null!;
    public string? Phone { get; set; } = null!;
    public string? ClientName { get; set; } = null!;
    public string? ClientSurname { get; set; } = null!;
    public string? ClientMiddleName { get; set; } = null!;
    public string? OrganizationName { get; set; } = null!;
    public string? WorkPosition { get; set; } = null!;
}