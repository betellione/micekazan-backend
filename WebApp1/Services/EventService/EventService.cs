using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;

namespace WebApp1.Services.EventService;

public class EventService(IQticketsApiProvider apiProvider, ApplicationDbContext context) : IEventService
{
    public async Task<bool> ImportEvents(Guid userId)
    {
        var token = await context.CreatorTokens.Where(x => x.CreatorId == userId).Select(x => x.Token).FirstOrDefaultAsync();
        if (token is null) return false;

        var eventModels = await apiProvider.GetEvents(token)
            .Select(x => new Event
            {
                Id = x.Id,
                CreatorId = userId,
                Name = x.Name,
                City = x.City.Name,
                CreatedAt = DateTime.UtcNow,
                StartedAt = x.Shows.Min(y => y.StartDate),
                FinishedAt = x.Shows.Max(y => y.FinishDate),
            })
            .ToListAsync();

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

    public async Task<bool> ImportEventTickets(long eventId)
    {
        var i = await Task.FromResult(0);
        return false;
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