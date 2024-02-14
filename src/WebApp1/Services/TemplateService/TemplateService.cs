using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp1.Data;
using WebApp1.Data.FileManager;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.ViewModels.Event;
using ILogger = Serilog.ILogger;

namespace WebApp1.Services.TemplateService;

public class TemplateService : ITemplateService
{
    private static readonly ImageSizeOptions HorizontalImageSizeOptions = new(1500, 1125);
    private static readonly ImageSizeOptions VerticalImageSizeOptions = new(1125, 1500);
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger = Log.ForContext<ITemplateService>();
    private readonly IServiceProvider _sp;

    public TemplateService(ApplicationDbContext context, IServiceProvider sp)
    {
        _context = context;
        _sp = sp;
    }

    public async Task<long> AddTemplate(Guid userId, TemplateViewModel vm)
    {
        var template = new TicketPdfTemplate
        {
            OrganizerId = userId,
            HasName = vm.DisplayName,
            HasSurname = vm.DisplaySurname,
            IsHorizontal = vm.PageOrientation == PageOrientation.Horizontal,
            TemplateName = string.Empty,
            TextColor = vm.FontColor,
            HasQrCode = vm.DisplayQrCode,
        };

        if (vm is { DeleteLogo: false, Logo: not null, })
        {
            var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
            template.LogoUri = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
        }

        if (vm is { DeleteBackground: false, Background: not null, })
        {
            var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");
            var options = vm.PageOrientation == PageOrientation.Horizontal ? HorizontalImageSizeOptions : VerticalImageSizeOptions;
            template.BackgroundUri = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName, options);
        }

        try
        {
            _context.TicketPdfTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template.Id;
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while adding template {Template} in DB", template);
            return 0;
        }
    }

    public async Task<TicketPdfTemplate?> GetTemplate(long id)
    {
        var template = await _context.TicketPdfTemplates.FindAsync(id);
        return template;
    }

    public async Task<TicketPdfTemplate?> GetTemplateForScanner(Guid scannerId)
    {
        var template = await _context.EventScanners
            .Where(x => x.ScannerId == scannerId)
            .Select(x => x.TicketPdfTemplate)
            .FirstOrDefaultAsync();

        if (template is null) return null;

        if (template.LogoUri is not null) template.LogoUri = Path.Combine("wwwroot-user", template.LogoUri);
        if (template.BackgroundUri is not null) template.BackgroundUri = Path.Combine("wwwroot-user", template.BackgroundUri);

        return template;
    }

    public async Task<IEnumerable<long>> GetTemplateIds(Guid userId)
    {
        var ids = await _context.TicketPdfTemplates.Where(x => x.OrganizerId == userId).OrderBy(x => x.Id).Select(x => x.Id).ToListAsync();
        return ids;
    }

    public async Task UpdateTemplate(TemplateViewModel vm)
    {
        if (vm.Id is null) return;

        var template = await _context.TicketPdfTemplates.FindAsync(vm.Id.Value);
        if (template is null) return;

        template.IsHorizontal = vm.PageOrientation == PageOrientation.Horizontal;
        template.HasName = vm.DisplayName;
        template.HasSurname = vm.DisplaySurname;
        template.HasQrCode = vm.DisplayQrCode;
        template.TextColor = vm.FontColor;

        await UpdateTemplateImages(template, vm);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            _logger.Error(e, "Error while updating template {Template} in DB", template);
        }
    }

    private async Task UpdateTemplateImages(TicketPdfTemplate template, TemplateViewModel vm)
    {
        var logoImageManager = _sp.GetRequiredKeyedService<IImageManager>("Logo");
        var backgroundImageManager = _sp.GetRequiredKeyedService<IImageManager>("Background");

        if (vm.DeleteLogo && template.LogoUri is not null)
        {
            logoImageManager.DeleteImage(Path.GetFileName(template.LogoUri));
            template.LogoUri = null;
        }
        else if (vm.Logo is not null)
        {
            if (template.LogoUri is null)
            {
                template.LogoUri = await logoImageManager.SaveImage(vm.Logo.OpenReadStream(), vm.Logo.FileName);
            }
            else
            {
                await logoImageManager.UpdateImage(Path.GetFileName(template.LogoUri), vm.Logo.OpenReadStream());
            }
        }

        if (vm.DeleteBackground && template.BackgroundUri is not null)
        {
            backgroundImageManager.DeleteImage(Path.GetFileName(template.BackgroundUri));
            template.BackgroundUri = null;
        }
        else if (vm.Background is not null)
        {
            var options = vm.PageOrientation == PageOrientation.Horizontal ? HorizontalImageSizeOptions : VerticalImageSizeOptions;
            if (template.BackgroundUri is null)
            {
                template.BackgroundUri = await backgroundImageManager.SaveImage(vm.Background.OpenReadStream(), vm.Background.FileName, options);
            }
            else
            {
                await backgroundImageManager.UpdateImage(Path.GetFileName(template.BackgroundUri), vm.Background.OpenReadStream(), options);
            }
        }
    }
}