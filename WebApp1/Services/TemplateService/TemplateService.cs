using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Data.FileManager;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Settings;

namespace WebApp1.Services.TemplateService;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _sp;

    public TemplateService(ApplicationDbContext context, IServiceProvider sp)
    {
        _context = context;
        _sp = sp;
    }

    public async Task<long> AddTemplate(Guid userId, TemplateViewModel vm)
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
            _context.TicketPdfTemplate.Add(template);
            await _context.SaveChangesAsync();
            return template.Id;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<TicketPdfTemplate?> GetTemplate(long id)
    {
        var template = await _context.TicketPdfTemplate.FindAsync(id);
        return template;
    }

    public async Task<IEnumerable<long>> GetTemplateIds(Guid userId)
    {
        var ids = await _context.TicketPdfTemplate.Where(x => x.OrganizerId == userId).OrderBy(x => x.Id).Select(x => x.Id).ToListAsync();
        return ids;
    }

    public async Task UpdateTemplate(TemplateViewModel vm)
    {
        if (vm.Id is null) return;

        var template = await _context.TicketPdfTemplate.FindAsync(vm.Id.Value);
        if (template is null) return;

        template.IsHorizontal = vm.PageOrientation == PageOrientation.Horizontal;
        template.HasName = vm.DisplayName;
        template.HasSurname = vm.DisplaySurname;
        template.HasQrCode = vm.DisplayQrCode;
        template.TextColor = vm.FontColor;

        if (vm.Logo is not null)
        {
            var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
            var logoPath = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
            template.LogoUri = logoPath;
        }

        if (vm.Background is not null)
        {
            var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");
            var backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName);
            template.BackgroundUri = backgroundPath;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}