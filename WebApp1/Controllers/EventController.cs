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
public class EventController(ApplicationDbContext context, IEventService eventService, IUserStore<User> userStore,
    UserManager<User> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = new Guid(userManager.GetUserId(User)!);

        if (!await context.CreatorTokens.AnyAsync(x => x.CreatorId == userId))
        {
            return RedirectToAction("Token", "Manage");
        }

        var applicationDbContext = context.Events.Include(x => x.Creator);

        return View(await applicationDbContext.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(long? id)
    {
        if (id is null) return BadRequest();

        var vm = await context.Events
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
                Scanners = x.Collectors.Select(y => new Scanner
                {
                    Id = y.CollectorId, Username = y.Collector.UserName!,
                }),
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return vm is null ? NotFound() : View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,City,CreatedAt,StartedAt,FinishedAt,CreatorId")] Event @event)
    {
        if (!ModelState.IsValid)
        {
            ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", @event.CreatorId);
            return View(@event);
        }

        context.Add(@event);
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id is null) return NotFound();

        var @event = await context.Events.FindAsync(id);
        if (@event is null) return NotFound();

        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", @event.CreatorId);

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
                context.Events.Update(@event);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(@event.Id)) return NotFound();
                throw;
            }

            return RedirectToAction("Index");
        }

        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", @event.CreatorId);

        return View(@event);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id is null) return NotFound();

        var @event = await context.Events
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
        var @event = await context.Events.FindAsync(id);
        if (@event is not null) context.Events.Remove(@event);

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private Task<bool> EventExists(long id)
    {
        return context.Events.AnyAsync(e => e.Id == id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventLoad()
    {
        var userId = new Guid(userManager.GetUserId(User)!);
        var hasToken = await context.CreatorTokens.AnyAsync(x => x.CreatorId == userId);

        if (!hasToken)
        {
            TempData["StatusMessage"] = "Введите свой токен в профиле";
            return RedirectToAction("Index");
        }

        var result = await eventService.ImportEvents(userId);
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
            NormalizedEmail = userManager.NormalizeEmail(vm.Email),
            EmailConfirmed = true,
        };

        await userStore.SetUserNameAsync(user, vm.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        var eventCollector = new EventCollector
        {
            CollectorId = user.Id,
            EventId = vm.EventId!.Value,
        };

        context.EventCollectors.Add(eventCollector);
        await context.SaveChangesAsync();
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, vm.Email));
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Scanner"));

        return RedirectToAction("Details", new { id = vm.EventId, });
    }
}