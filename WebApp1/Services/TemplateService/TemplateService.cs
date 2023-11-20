using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Data.FileManager;
using WebApp1.Models;
using WebApp1.Types;
using WebApp1.ViewModels.Settings;

namespace WebApp1.Services.TemplateService;

public class TemplateService(ApplicationDbContext context, IServiceProvider sp) : ITemplateService
{
    public async Task<long> AddTemplate(Guid userId, TemplateViewModel vm)
    {
        string? logoPath = null, backgroundPath = null;

        if (vm.Logo is not null)
        {
            var logoImageManager = sp.GetRequiredKeyedService<IImageManager>("Logo");
            logoPath = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
        }

        if (vm.Background is not null)
        {
            var backgroundImageManager = sp.GetRequiredKeyedService<IImageManager>("Background");
            backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
        }

        var template = new TicketPdfTemplate
        {
            BackgroundUri = backgroundPath,
            LogoUri = logoPath,
            OrganizerId = userId,
            HasName = vm.DisplayName,
            HasSurname = vm.DisplaySurname,
            IsHorizontal = vm.PageOrientation == PageOrientation.Horizontal,
            TemplateName = string.Empty,
            TextColor = vm.FontColor,
            HasQrCode = vm.DisplayQrCode,
        };

        try
        {
            context.TicketPdfTemplate.Add(template);
            await context.SaveChangesAsync();
            return template.Id;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<TicketPdfTemplate?> GetTemplate(long id)
    {
        var template = await context.TicketPdfTemplate.FindAsync(id);
        return template;
    }

    public async Task<IEnumerable<long>> GetTemplateIds(Guid userId)
    {
        var ids = await context.TicketPdfTemplate.Where(x => x.OrganizerId == userId).OrderBy(x => x.Id).Select(x => x.Id).ToListAsync();
        return ids;
    }

    public async Task UpdateTemplate(TemplateViewModel vm)
    {
        if (vm.Id is null) return;

        var template = await context.TicketPdfTemplate.FindAsync(vm.Id.Value);
        if (template is null) return;

        template.IsHorizontal = vm.PageOrientation == PageOrientation.Horizontal;
        template.HasName = vm.DisplayName;
        template.HasSurname = vm.DisplaySurname;
        template.HasQrCode = vm.DisplayQrCode;
        template.TextColor = vm.FontColor;

        if (vm.Logo is not null)
        {
            var logoImageManager = sp.GetRequiredKeyedService<IImageManager>("Logo");
            var logoPath = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
            template.LogoUri = logoPath;
        }

        if (vm.Background is not null)
        {
            var backgroundImageManager = sp.GetRequiredKeyedService<IImageManager>("Background");
            var backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
            template.BackgroundUri = backgroundPath;
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}