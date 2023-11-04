using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer")]
public class EventController(ApplicationDbContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = context.Events.Include(x => x.Creator);
        return View(await applicationDbContext.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null) return NotFound();

        var @event = await context.Events
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null) return NotFound();

        return View(@event);
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
        if (ModelState.IsValid)
        {
            context.Add(@event);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", @event.CreatorId);
        return View(@event);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null) return NotFound();

        var @event = await context.Events.FindAsync(id);
        if (@event == null) return NotFound();

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
                context.Update(@event);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(@event.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CreatorId"] = new SelectList(context.Users, "Id", "Id", @event.CreatorId);
        return View(@event);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null) return NotFound();

        var @event = await context.Events
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null) return NotFound();

        return View(@event);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var @event = await context.Events.FindAsync(id);
        if (@event != null) context.Events.Remove(@event);

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(long id)
    {
        return context.Events.Any(e => e.Id == id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EventLoad()
    {
        TempData["StatusMessage"] = "Чамарчик превратился в баребулу (все хорошо).";
        return RedirectToAction("Index");
    }
}