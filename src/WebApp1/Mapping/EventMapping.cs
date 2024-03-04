using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Mapping;

public static class EventMapping
{
    public static ScreenViewModel MapToEventDisplay(this Screen screen)
    {
        return new ScreenViewModel
        {
            Id = screen.Id,
            MainText = screen.WelcomeText,
            Description = screen.Description,
            LogoPath = screen.LogoUri,
            BackgroundPath = screen.BackgroundUri,
            BackgroundColor = screen.BackgroundColor,
            TextColor = screen.TextColor,
            TextSize = screen.TextSize,
        };
    }

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