using System.ComponentModel;

namespace WebApp1.ViewModels.Event;

public class ScreenViewModel
{
    public long Id { get; set; }

    [DisplayName("Главный текст")]
    public string? MainText { get; set; }

    [DisplayName("Описание")]
    public string? Description { get; set; }
    

    [DisplayName("Загрузить логотип")]
    public IFormFile? Logo { get; set; }

    [DisplayName("Загрузить фон")]
    public IFormFile? Background { get; set; }
    
    public string TextColor { get; set; } = "#000000";
    public int TextSize { get; set; } = 70;
    public string BackgroundColor { get; set; } = "#FFFFFF";

    public string? LogoPath { get; set; }
    public string? BackgroundPath { get; set; }
    
    public bool LogoDeleted { get; set; }
    public bool BackgroundDeleted { get; set; } = false;
}