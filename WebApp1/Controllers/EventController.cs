using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.ViewModels;
using WebApp1.ViewModels.Event;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer", Policy = "RegisterConfirmation")]
public class EventController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEventService _eventService;
    private readonly IUserStore<User> _userStore;
    private readonly UserManager<User> _userManager;

    public EventController(ApplicationDbContext context, IEventService eventService, IUserStore<User> userStore,
        UserManager<User> userManager)
    {
        _context = context;
        _eventService = eventService;
        _userStore = userStore;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        if (!await _context.CreatorTokens.AnyAsync(x => x.CreatorId == userId))
        {
            return RedirectToAction("Token", "Manage");
        }

        var applicationDbContext = _context.Events.Include(x => x.Creator);

        return View(await applicationDbContext.ToListAsync());
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
        var vm = new UserViewModel { EventId = eventId, EventName = eventName, };
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
            return View(vm);
        }

        var eventCollector = new EventScanner
        {
            ScannerId = user.Id,
            EventId = vm.EventId!.Value,
            PrintingToken = vm.Token,
        };

        _context.EventScanners.Add(eventCollector);
        await _context.SaveChangesAsync();
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, vm.Email));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Scanner"));

        return RedirectToAction("Details", new { id = vm.EventId, });
    }
}