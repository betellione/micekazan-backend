using System.ComponentModel;

namespace WebApp1.ViewModels.Settings;

public class ScreenViewModel
{
    public long? Id { get; set; }

    [DisplayName("Главный текст")]
    public string? MainText { get; set; }

    [DisplayName("Описание")]
    public string? Description { get; set; }
    

    [DisplayName("Загрузить логотип")]
    public IFormFile? Logo { get; set; }

    [DisplayName("Загрузить фон")]
    public IFormFile? Background { get; set; }
    
    public string BackgroundColor { get; set; } = "#000000";

    public string? LogoPath { get; set; }
    public string? BackgroundPath { get; set; }
}