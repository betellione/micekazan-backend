namespace WebApp1.ViewModels.Settings;

public class IndexViewModel
{
    public string TemplateName { get; set; } = null!;
    public string TextColor { get; set; } = "#000000";
    public bool IsHorizontal { get; set; } = true;
    public bool HasName { get; set; } = true;
    public bool HasSurname { get; set; } = true;
    public bool HasQrCode { get; set; } = true;
}