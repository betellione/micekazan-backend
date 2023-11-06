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
}