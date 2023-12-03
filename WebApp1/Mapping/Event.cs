using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Mapping;

public static class Event
{
    public static EventDetails MapToEventDetails(this Models.Event @event, IEnumerable<User> scanners)
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
}