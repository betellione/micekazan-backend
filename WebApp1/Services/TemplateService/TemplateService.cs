using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Data.FileManager;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;

namespace WebApp1.Services.TemplateService;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _sp;
    private static readonly ImageSizeOptions HorizontalImageSizeOptions = new(1500, 1125);
    private static readonly ImageSizeOptions VerticalImageSizeOptions = new(1125, 1500);

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
            var options = vm.PageOrientation == PageOrientation.Horizontal ? HorizontalImageSizeOptions : VerticalImageSizeOptions;
            backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName, options);
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

    public async Task<TicketPdfTemplate?> GetTemplateForScanner(Guid scannerId)
    {
        var template = await _context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .Select(x => x.TicketPdfTemplate)
            .FirstOrDefaultAsync();

        if (template is null) return null;

        if (template.LogoUri is not null) template.LogoUri = Path.Combine("wwwroot", template.LogoUri);
        if (template.BackgroundUri is not null) template.BackgroundUri = Path.Combine("wwwroot", template.BackgroundUri);

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
            var options = vm.PageOrientation == PageOrientation.Horizontal ? HorizontalImageSizeOptions : VerticalImageSizeOptions;
            var backgroundPath = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName, options);
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