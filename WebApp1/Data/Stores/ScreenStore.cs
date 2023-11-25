using Microsoft.EntityFrameworkCore;
using WebApp1.Enums;
using WebApp1.Models;

namespace WebApp1.Data.Stores;

public class ScreenStore: IScreenStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ScreenStore(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Screen?> GetScreenByType(long eventId, ScreenTypes type)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var screen = await context.Screen.FirstOrDefaultAsync(x => x.EventId == eventId && x.Type == type);
        return screen;
    }
}