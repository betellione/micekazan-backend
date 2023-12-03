using Microsoft.EntityFrameworkCore;
using WebApp1.Data.FileManager;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Data.Stores;

public class ScreenStore: IScreenStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IServiceProvider _sp;

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

    public async Task<long> AddTemplate(long eventId, ScreenViewModel vm, ScreenTypes type)
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

        var template = new Screen()
        {
            BackgroundUri = backgroundPath,
            LogoUri = logoPath,
            WelcomeText = vm.MainText!,
            Description = vm.Description!,
            TextColor = vm.BackgroundColor,
            Type = type,
            EventId = eventId,
            
        };
        await using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            context.Screen.Add(template);
            await context.SaveChangesAsync();
            return template.Id;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}