using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer", Policy = "RegisterConfirmation")]
public class ClientController(ApplicationDbContext context, UserManager<User> userManager) : Controller
{
    // GET: Client
    public async Task<IActionResult> Index()
    {
        var userId = new Guid(userManager.GetUserId(User)!);

        if (!await context.CreatorTokens.AnyAsync(x => x.CreatorId == userId))
        {
            return RedirectToAction("Token", "Manage");
        }
        return View(await context.Clients.ToListAsync());
    }

    // GET: Client/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await context.Clients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // GET: Client/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Client/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Surname,Patronymic,Email,PhoneNumber")] Client client)
    {
        if (ModelState.IsValid)
        {
            context.Add(client);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(client);
    }

    // GET: Client/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // POST: Client/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Surname,Patronymic,Email,PhoneNumber")] Client client)
    {
        if (id != client.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(client);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(client.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(client);
    }

    // GET: Client/Delete/5
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await context.Clients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // POST: Client/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var client = await context.Clients.FindAsync(id);
        if (client != null)
        {
            context.Clients.Remove(client);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClientExists(long id)
    {
        return context.Clients.Any(e => e.Id == id);
    }
}