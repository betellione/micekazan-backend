using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Data.Stores;
using WebApp1.Enums;
using WebApp1.Mapping;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.Services.TemplateService;
using WebApp1.ViewModels;
using WebApp1.ViewModels.Event;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer", Policy = "RegisterConfirmation")]
public class EventController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IScreenStore _screenStore;
    private readonly IScannerStore _scannerStore;
    private readonly IEventService _eventService;
    private readonly IUserStore<User> _userStore;
    private readonly UserManager<User> _userManager;
    private readonly ITemplateService _templateService;

    public EventController(ApplicationDbContext context, IScreenStore screenStore, IScannerStore scannerStore, IEventService eventService, IUserStore<User> userStore,
        UserManager<User> userManager, ITemplateService templateService)
    {
        _scannerStore = scannerStore;
        _screenStore = screenStore;
        _context = context;
        _eventService = eventService;
        _userStore = userStore;
        _userManager = userManager;
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string sortOrder)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        if (!await _context.CreatorTokens.AnyAsync(x => x.CreatorId == userId))
        {
            return RedirectToAction("Token", "Manage");
        }

        var applicationDbContext = _context.Events.Include(x => x.Creator);
        var orderedQueryable = sortOrder switch
        {
            "name" => applicationDbContext.OrderBy(x => x.Name),
            "city" => applicationDbContext.OrderBy(x => x.City),
            "start" => applicationDbContext.OrderBy(x => x.StartedAt),
            "finish" => applicationDbContext.OrderBy(x => x.FinishedAt),
            _ => applicationDbContext.OrderByDescending(x => x.StartedAt)
        };

        return View(await orderedQueryable.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(long? id)
    {
        if (id is null) return BadRequest();

        var vm = await _context.Events
            .Select(x => new EventDetails
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                CreatedAt = x.CreatedAt,
                StartedAt = x.StartedAt,
                FinishedAt = x.FinishedAt,
                CreatorId = x.CreatorId,
                CreatorUsername = x.Creator.UserName!,
                AllTickets = _context.Tickets.Where(q => q.PassedAt != null)
                    .Count(t => t.Event == _context.Events.FirstOrDefault(e => e.Id == id)),
                Scanners = x.Collectors.Select(y => new Scanner
                {
                    Id = y.ScannerId, Username = y.Scanner.UserName!,
                }),
                PassedTickets = x.Tickets.Where(t => t.PassedAt != null).Select(y => new PassedTickets
                {
                    Id = y.Id, Name = y.Client.Name, Surname = y.Client.Surname, Patronymic = y.Client.Patronymic, PassedAt = y.PassedAt!.Value,
                }),
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return vm is null ? NotFound() : View(vm);
    }
    
    [HttpGet]
    public async Task<IActionResult>Scanners(long? id)
    {
        if (id is null) return BadRequest();

        var vm = await _context.Events
            .Select(x => new Scanners
            {
                Id = x.Id,
                EventName = x.Name,
                EventId = x.Id,
                ScannersList = x.Collectors.Select(y => new Scanner
                {
                    Id = y.ScannerId, Username = y.Scanner.UserName!,
                }),
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return vm is null ? NotFound() : View(vm);
    }
    
    [HttpGet]
    public async Task<IActionResult>Statistics(long? id)
    {
        if (id is null) return BadRequest();

        var vm = await _context.Events
            .Select(x => new Tickets
            {
                Id = x.Id,
                EventId = x.Id,
                PassedTickets = x.Tickets.Where(t => t.PassedAt != null).Select(y => new PassedTickets
                {
                    Id = y.Id, Name = y.Client.Name, Surname = y.Client.Surname, Patronymic = y.Client.Patronymic, PassedAt = y.PassedAt!.Value,
                }),
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return vm is null ? NotFound() : View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,City,CreatedAt,StartedAt,FinishedAt,CreatorId")] Event @event)
    {
        if (!ModelState.IsValid)
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", @event.CreatorId);
            return View(@event);
        }

        _context.Add(@event);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id is null) return NotFound();

        var @event = await _context.Events.FindAsync(id);
        if (@event is null) return NotFound();

        ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", @event.CreatorId);

        return View(@event);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Name,City,CreatedAt,StartedAt,FinishedAt,CreatorId")] Event @event)
    {
        if (id != @event.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Events.Update(@event);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(@event.Id)) return NotFound();
                throw;
            }

            return RedirectToAction("Index");
        }

        ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", @event.CreatorId);

        return View(@event);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id is null) return NotFound();

        var @event = await _context.Events
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (@event is null) return NotFound();

        return View(@event);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event is not null) _context.Events.Remove(@event);

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private Task<bool> EventExists(long id)
    {
        return _context.Events.AnyAsync(e => e.Id == id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventLoad()
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var hasToken = await _context.CreatorTokens.AnyAsync(x => x.CreatorId == userId);

        if (!hasToken)
        {
            TempData["StatusMessage"] = "Введите свой токен в профиле";
            return RedirectToAction("Index");
        }

        var result = await _eventService.ImportEvents(userId);
        TempData["StatusMessage"] = result ? "События успешно загружены" : "Произошла ошибка при загрузке событий";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult AddScanner(long? eventId, string? eventName)
    {
        var vm = new UserViewModel { EventId = eventId, EventName = eventName};
        vm = FillUpMyViewModel(vm);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddScanner(UserViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new User
        {
            Email = vm.Email,
            NormalizedEmail = _userManager.NormalizeEmail(vm.Email),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        await _userStore.SetUserNameAsync(user, vm.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            vm = FillUpMyViewModel(vm);
            return View(vm);
        }

        _ = long.TryParse(vm.SelectedTemplateId, out var ticketPdfTemplateId);
        var eventCollector = new EventScanner
        {
            ScannerId = user.Id,
            EventId = vm.EventId!.Value,
            PrintingToken = vm.Token,
            TicketPdfTemplateId = ticketPdfTemplateId
        };

        _context.EventScanners.Add(eventCollector);
        await _context.SaveChangesAsync();
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, vm.Email));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Scanner"));
        if (vm.IsAutomate)
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Actor, "Automate"));
        

        return RedirectToAction("Details", new { id = vm.EventId, });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditScanner(long? eventId, string? eventName, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var scanner = await _scannerStore.FindScannerById(userId);
        if (user is null || scanner is null) return NotFound();
        var isAutomate = (await _userManager.GetClaimsAsync(user)).Any(x => x.Value == "Automate");

        var vm = new EditScannerViewModel
        {
            Id = userId,
            Token = scanner.PrintingToken,
            SelectedTemplateId = scanner.TicketPdfTemplateId.ToString(),
            IsAutomate = isAutomate,
            EventId = eventId,
            EventName = eventName
        };
        vm = FillUpMyEditViewModel(vm);
        return View(vm);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditScanner(EditScannerViewModel vm)
    {
        var user = await _userManager.FindByIdAsync(vm.Id.ToString());
        var scanner = await _context.EventScanners.FirstOrDefaultAsync(x => x.ScannerId == vm.Id);
        if (user is null || scanner is null) return NotFound();
        
        _ = long.TryParse(vm.SelectedTemplateId, out var ticketPdfTemplateId);
        scanner.TicketPdfTemplateId = ticketPdfTemplateId;
        scanner.PrintingToken = vm.Token;
        if (await _scannerStore.SetClaimsForScanner(vm.Id, vm.IsAutomate))
            await _userManager.UpdateSecurityStampAsync(user);
        
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = vm.EventId, });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditDisplay(long eventId)
    {
        var waiting = await _screenStore.GetScreenByType(eventId, ScreenTypes.Waiting) ?? new Screen();
        var success = await _screenStore.GetScreenByType(eventId, ScreenTypes.Success) ?? new Screen();
        var fail = await _screenStore.GetScreenByType(eventId, ScreenTypes.Fail) ?? new Screen();
        var vm = new DisplayViewModel
        {
            EventId = eventId,
            WaitingDisplayViewModel = waiting.MapToEventDisplay(),
            SuccessDisplayViewModel = success.MapToEventDisplay(),
            FailDisplayViewModel = fail.MapToEventDisplay(),
        };
        return View(vm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDisplay(DisplayViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await _screenStore.AddOrUpdateScreen(vm.EventId, vm.WaitingDisplayViewModel, ScreenTypes.Waiting);
        await _screenStore.AddOrUpdateScreen(vm.EventId, vm.SuccessDisplayViewModel, ScreenTypes.Success);
        await _screenStore.AddOrUpdateScreen(vm.EventId, vm.FailDisplayViewModel, ScreenTypes.Fail);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Print(long eventId, long? templateId)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var template = templateId is null
            ? new TemplateViewModel{EventId = eventId}
            : (await _templateService.GetTemplate(templateId.Value))?.MapToViewModel();

        var vm = new PrintViewModel
        {
            EventId = eventId,
            TemplateViewModel = template ?? new TemplateViewModel{EventId = eventId},
            TemplateIds = await _templateService.GetTemplateIds(userId),
            SelectedTemplateId = templateId,
        };
        return View(vm);
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTemplateAsNew(TemplateViewModel vm)
    {
        if (!ModelState.IsValid) return View("Print", new PrintViewModel { TemplateViewModel = vm, });

        var userId = new Guid(_userManager.GetUserId(User)!);
        await _templateService.AddTemplate(userId, vm);

        return RedirectToAction("Print", new { vm.EventId, });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTemplate(TemplateViewModel vm)
    {
        if (!ModelState.IsValid) return View("Print", new PrintViewModel { TemplateViewModel = vm, });

        await _templateService.UpdateTemplate(vm);

        return RedirectToAction("Print", new { templateId = vm.Id, vm.EventId, });
    }

    private UserViewModel FillUpMyViewModel(UserViewModel vm)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var templates = _context.TicketPdfTemplate
            .Where(x => x.OrganizerId == userId)
            .Select(x => new SelectListItem($"Шаблон {x.Id}", x.Id.ToString()));
        vm.TemplateIds = templates;
        return vm;
    }
    
    private EditScannerViewModel FillUpMyEditViewModel(EditScannerViewModel vm)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);
        var templates = _context.TicketPdfTemplate
            .Where(x => x.OrganizerId == userId)
            .Select(x => new SelectListItem($"Шаблон {x.Id}", x.Id.ToString()));
        vm.TemplateIds = templates;
        return vm;
    }
}