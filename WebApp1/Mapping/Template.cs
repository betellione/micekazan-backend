using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Settings;

namespace WebApp1.Mapping;

public static class Template
{
    public static TemplateViewModel MapToViewModel(this TicketPdfTemplate model)
    {
        return new TemplateViewModel
        {
            Id = model.Id,
            FontColor = model.TextColor,
            DisplayName = model.HasName,
            DisplaySurname = model.HasSurname,
            DisplayQrCode = model.HasQrCode,
            PageOrientation = model.IsHorizontal ? PageOrientation.Horizontal : PageOrientation.Vertical,
            LogoPath = model.LogoUri,
            BackgroundPath = model.BackgroundUri,
        };
    }
}