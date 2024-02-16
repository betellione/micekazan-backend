using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.Data.Stores;
using WebApp1.External.Qtickets;
using WebApp1.Models;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.EventService;

public class EventService : IEventService
{
    private readonly IQticketsApiProvider _apiProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger _logger = Log.ForContext<IEventService>();
    private readonly ITokenStore _tokenStore;

    public EventService(IDbContextFactory<ApplicationDbContext> contextFactory, ITokenStore tokenStore,
        IQticketsApiProvider apiProvider)
    {
        _contextFactory = contextFactory;
        _tokenStore = tokenStore;
        _apiProvider = apiProvider;
    }

    public async Task<bool> ImportEvents(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var token = await _tokenStore.GetCurrentOrganizerToken(userId);
        if (token is null) return false;

        var events = _apiProvider.GetEvents(token, cancellationToken)
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
        var existed = (await context.Events.Select(x => x.Id).ToListAsync(cancellationToken)).ToHashSet();

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

            if (++batchCounter < 100) continue;
            batchCounter = 0;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while saving events in the DB");
            }
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while saving events in the DB");
        }

        return true;
    }

    public async Task<Event?> GetById(long id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var events = await context.Events.FirstAsync(x => x.Id == id);
        return events;
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

    public async Task<int> GetAllTicketsNumber(Guid scannerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var allTicketsNumber = await context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .SelectMany(x => x.Event.Tickets)
            .CountAsync();
        return allTicketsNumber;
    }

    public async Task<int> GetScannedTicketsNumber(Guid scannerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var scannedTickets = await context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .SelectMany(x => x.Event.Tickets)
            .Where(x => x.PassedAt != null)
            .CountAsync();
        return scannedTickets;
    }

    public async Task<int> GetAllTicketsNumber(long eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var allTicketsNumber = await context.Events
            .SelectMany(x => x.Tickets)
            .CountAsync();
        return allTicketsNumber;
    }

    public async Task<int> GetScannedTicketsNumber(long eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var allTicketsNumber = await context.Events
            .SelectMany(x => x.Tickets).Where(x => x.PassedAt != null)
            .CountAsync();
        return allTicketsNumber;
    }
}