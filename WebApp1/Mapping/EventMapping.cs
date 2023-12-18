using WebApp1.Models;
using WebApp1.ViewModels.Event;
using WebApp1.Enums;

namespace WebApp1.Mapping;

public static class EventMapping
{
    public static EventDetails MapToEventDetails(this Event @event, IEnumerable<User> scanners)
    {
        return new EventDetails
        {
            Id = @event.Id,
            Name = @event.Name,
            City = @event.City,
            CreatedAt = @event.CreatedAt,
            StartedAt = @event.StartedAt,
            FinishedAt = @event.FinishedAt,
            CreatorId = @event.CreatorId,
            CreatorUsername = @event.Creator.UserName!,
            Scanners = scanners.Select(x => x.MapToScanner()),
        };
    }
    
    public static ScreenViewModel MapToEventDisplay(this Screen @screen)
    {
        return new ScreenViewModel
        {
            Id = @screen.Id,
            MainText = @screen.WelcomeText,
            Description = @screen.Description,
            LogoPath = @screen.LogoUri,
            BackgroundPath = @screen.BackgroundUri,
            BackgroundColor = @screen.TextColor,
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