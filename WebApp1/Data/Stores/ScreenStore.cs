using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data.FileManager;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;
using ILogger = Serilog.ILogger;

namespace WebApp1.Data.Stores;

public class ScreenStore : IScreenStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IServiceProvider _sp;
    private readonly ILogger _logger = Log.ForContext<IScreenStore>();

    public ScreenStore(IDbContextFactory<ApplicationDbContext> contextFactory, IServiceProvider sp)
    {
        _contextFactory = contextFactory;
        _sp = sp;
    }

    public async Task<Screen?> GetScreenByType(long eventId, ScreenTypes type)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var screen = await context.Screen.FirstOrDefaultAsync(x => x.EventId == eventId && x.Type == type);
        return screen;
    }

    public async Task<Screen> AddOrUpdateScreen(long eventId, ScreenViewModel vm, ScreenTypes type)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var screen = await context.Screen.FirstOrDefaultAsync(x => x.EventId == eventId && x.Type == type);
        if (screen is null) return await AddScreen(context, eventId, vm, type);
        return await UpdateScreen(context, screen, vm);
    }

    private async Task<Screen> UpdateScreen(ApplicationDbContext context, Screen screen, ScreenViewModel vm)
    {
        screen.WelcomeText = vm.MainText ?? string.Empty;
        screen.Description = vm.Description ?? string.Empty;
        screen.TextColor = vm.BackgroundColor;
        screen.LogoUri = vm.LogoPath;
        screen.BackgroundUri = vm.BackgroundPath;

        try
        {
            await context.SaveChangesAsync();
            return screen;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Паша пидор потому что:");
            throw;
        }
    }

    private async Task<Screen> AddScreen(ApplicationDbContext context, long eventId, ScreenViewModel vm, ScreenTypes type)
    {
        string? logoPath = null, backgroundPath = null;

        if (vm.Logo is not null)
        {
            var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
            logoPath = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
        }

        if (vm.Background is not null)
        {
            var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");
            backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
        }

        var screen = new Screen
        {
            BackgroundUri = backgroundPath,
            LogoUri = logoPath,
            WelcomeText = vm.MainText!,
            Description = vm.Description!,
            TextColor = vm.BackgroundColor,
            Type = type,
            EventId = eventId,
        };
        try
        {
            context.Screen.Add(screen);
            await context.SaveChangesAsync();
            return screen;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Паша пидор потому что:");
            throw;
        }
    }
}