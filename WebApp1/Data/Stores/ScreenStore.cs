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
    private readonly ILogger _logger = Log.ForContext<IScreenStore>();
    private readonly IServiceProvider _sp;

    public ScreenStore(IDbContextFactory<ApplicationDbContext> contextFactory, IServiceProvider sp)
    {
        _contextFactory = contextFactory;
        _sp = sp;
    }

    public async Task<Screen?> GetScreenByType(long eventId, ScreenTypes type)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await GetScreenByType(context, eventId, type);
    }

    public async Task<Screen> AddOrUpdateScreen(long eventId, ScreenViewModel vm, ScreenTypes type)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var screen = await GetScreenByType(context, eventId, type);
        return screen is null ? await AddScreen(context, eventId, vm, type) : await UpdateScreen(context, screen, vm);
    }

    private static Task<Screen?> GetScreenByType(ApplicationDbContext context, long eventId, ScreenTypes type)
    {
        return context.Screens.FirstOrDefaultAsync(x => x.EventId == eventId && x.Type == type);
    }

    private async Task UpdateScreenImages(Screen screen, ScreenViewModel vm)
    {
        var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
        var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");

        if (vm.DeleteLogo && screen.LogoUri is not null)
        {
            logoImageManager.DeleteImage(Path.GetFileName(screen.LogoUri));
            screen.LogoUri = null;
        }
        else if (vm.Logo is not null)
        {
            if (screen.LogoUri is null)
            {
                screen.LogoUri = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
            }
            else
            {
                await logoImageManager.UpdateImage(Path.GetFileName(screen.LogoUri), vm.Logo.OpenReadStream());
            }
        }

        if (vm.DeleteBackground && screen.BackgroundUri is not null)
        {
            backgroundImageManager.DeleteImage(Path.GetFileName(screen.BackgroundUri));
            screen.BackgroundUri = null;
        }
        else if (vm.Background is not null)
        {
            if (screen.BackgroundUri is null)
            {
                screen.BackgroundUri = await logoImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
            }
            else
            {
                await backgroundImageManager.UpdateImage(Path.GetFileName(screen.BackgroundUri), vm.Background.OpenReadStream());
            }
        }
    }

    private async Task<Screen> UpdateScreen(ApplicationDbContext context, Screen screen, ScreenViewModel vm)
    {
        screen.WelcomeText = vm.MainText;
        screen.Description = vm.Description;
        screen.BackgroundColor = vm.BackgroundColor;
        screen.TextColor = vm.TextColor;
        screen.TextSize = vm.TextSize;

        await UpdateScreenImages(screen, vm);

        try
        {
            await context.SaveChangesAsync();
            return screen;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error while updating screen {Screen} in DB with view model {ViewModel}", screen, vm);
            throw;
        }
    }

    private async Task<Screen> AddScreen(ApplicationDbContext context, long eventId, ScreenViewModel vm, ScreenTypes type)
    {
        var screen = new Screen
        {
            WelcomeText = vm.MainText,
            Description = vm.Description,
            TextColor = vm.TextColor,
            BackgroundColor = vm.BackgroundColor,
            TextSize = vm.TextSize,
            Type = type,
            EventId = eventId,
        };

        if (vm.Logo is not null)
        {
            var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
            screen.LogoUri = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
        }

        if (vm.Background is not null)
        {
            var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");
            screen.BackgroundUri = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
        }

        try
        {
            context.Screens.Add(screen);
            await context.SaveChangesAsync();
            return screen;
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while adding screen {Screen} in DB", screen);
            throw;
        }
    }
}