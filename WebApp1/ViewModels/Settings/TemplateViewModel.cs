using System.ComponentModel;
using WebApp1.Enums;

namespace WebApp1.ViewModels.Settings;

public class TemplateViewModel
{
    public long? Id { get; set; }
    public string FontColor { get; set; } = "#000000";
    public PageOrientation PageOrientation { get; set; }

    [DisplayName("Имя")]
    public bool DisplayName { get; set; }

    [DisplayName("Фамилия")]
    public bool DisplaySurname { get; set; }

    [DisplayName("QR-код с данными")]
    public bool DisplayQrCode { get; set; }

    [DisplayName("Загрузить логотип")]
    public IFormFile? Logo { get; set; }

    [DisplayName("Загрузить фон")]
    public IFormFile? Background { get; set; }

    public string? LogoPath { get; set; }
    public string? BackgroundPath { get; set; }
}