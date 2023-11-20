namespace WebApp1.Services.PdfGenerator;

public class TicketDocumentModel
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string FontColor { get; set; } = "#000000";
    public string? BackgroundPath { get; set; }
    public string? LogoPath { get; set; }
    public string? QrPath { get; set; }
    public bool IsHorizontal { get; set; }
}