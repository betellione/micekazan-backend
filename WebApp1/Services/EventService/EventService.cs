using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using WebApp1.Services.TokenService;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.EventService;

public class EventService : IEventService
{
    private readonly ILogger _logger = Log.ForContext<IEventService>();
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ITokenService _tokenService;
    private readonly IQticketsApiProvider _apiProvider;

    public EventService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenService tokenService,
        IQticketsApiProvider apiProvider)
    {
        _contextFactory = contextFactory;
        _tokenService = tokenService;
        _apiProvider = apiProvider;
    }

    public async Task<bool> ImportEvents(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var token = await _tokenService.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var events = _apiProvider.GetEvents(token)
            .Select(x => new Event
            {
                Id = x.Id,
                CreatorId = userId,
                Name = x.Name,
                City = x.City.Name,
                CreatedAt = DateTime.UtcNow,
                StartedAt = x.Shows.Min(y => y.StartDate),
                FinishedAt = x.Shows.Max(y => y.FinishDate),
                ForeignShowIds = x.ShowIds.ToArray(),
            });

        var batchCounter = 0;
        var existed = (await context.Events.Select(x => x.Id).ToListAsync()).ToHashSet();

        await foreach (var @event in events)
        {
            if (existed.Contains(@event.Id))
            {
                context.Events.Update(@event);
            }
            else
            {
                context.Events.Add(@event);
            }

            try
            {
                if (++batchCounter >= 100)
                {
                    batchCounter = 0;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving events in the DB");
            }
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while saving events in the DB");
        }

        return true;
    }

    public Task GetById()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Event>> GetAll(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var events = await context.Events
            .Where(x => x.CreatorId == userId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync();
        return events;
    }
}