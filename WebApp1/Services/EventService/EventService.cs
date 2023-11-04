using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;

namespace WebApp1.Services.EventService;

public class EventService(IQticketsApiProvider apiProvider, ApplicationDbContext context) : IEventService
{
    public async Task<bool> ImportEvents(Guid userId)
    {
        var creatorToken = await context.CreatorTokens.FirstOrDefaultAsync(x => x.CreatorId == userId);
        if (creatorToken is null) return false;
        
        var events = (await apiProvider.GetActiveEvents(creatorToken.Token)).ToList();

        var eventModels = events.Select(x => new Event
        {
            Id = x.Id,
            CreatorId = userId,
            Name = x.Name,
            City = x.City.Name,
            CreatedAt = DateTime.UtcNow,
            StartedAt = x.Shows.Min(y => y.StartDate),
            FinishedAt = x.Shows.Max(y => y.FinishDate),
        });
        context.Events.AddRange(eventModels);

        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public Task GetById()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Event>> GetAll(Guid userId)
    {
        var events = await context.Events
            .Where(x => x.CreatorId == userId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
        return events;
    }
}