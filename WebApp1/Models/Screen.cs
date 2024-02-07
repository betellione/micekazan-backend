using WebApp1.Enums;

namespace WebApp1.Models;

public class Screen
{
    public long Id { get; set; }
    public ScreenTypes Type { get; set; } = ScreenTypes.Waiting;
    public string? WelcomeText { get; set; }
    public string? Description { get; set; }
    public string TextColor { get; set; } = "#000000";
    public int TextSize { get; set; } = 70;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    
    public string? LogoUri { get; set; }
    public string? BackgroundUri { get; set; }
    public long EventId { get; set; }

    public Event Event { get; set; } = null!;
}