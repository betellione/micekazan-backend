namespace WebApp1.Models;

public class TicketPdfTemplate
{
    public long Id { get; set; }
    public string TemplateName { get; set; } = null!;
    public string TextColor { get; set; } = "#000000";
    public bool IsHorizontal { get; set; } = true;
    public bool HasName { get; set; } = true;
    public bool HasSurname { get; set; } = true;
    public bool HasQrCode { get; set; } = true;
    public string? LogoUri { get; set; }
    public string? BackgroundUri { get; set; }
    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;
}