using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.Services.ClientService;

namespace WebApp1.Controllers;

[Authorize(Roles = "Organizer", Policy = "RegisterConfirmation")]
public class ClientController : Controller
{
    private readonly IClientService _clientService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ClientController(ApplicationDbContext context, UserManager<User> userManager, IClientService clientService)
    {
        _context = context;
        _userManager = userManager;
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        if (!await _context.CreatorTokens.AnyAsync(x => x.CreatorId == userId))
        {
            return RedirectToAction("Token", "Manage");
        }

        return View(await _context.Clients.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Surname,Patronymic,Email,PhoneNumber")] Client client)
    {
        if (ModelState.IsValid)
        {
            _context.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(client);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

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
                _context.Update(client);
                await _context.SaveChangesAsync();
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

    [HttpGet]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = await _context.Clients
            .FirstOrDefaultAsync(m => m.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client != null)
        {
            _context.Clients.Remove(client);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [HttpGet("[controller]/{token}")]
    public async Task<IActionResult> InfoToShow(string token)
    {
        var data = await _clientService.GetClientData(token);
        return View(data);
    }
    
    [AllowAnonymous]
    public IActionResult DownloadVCard(string fullName, string phone, string organization, string position)
    {
        var vCardText = new StringBuilder();
        vCardText.AppendLine("BEGIN:VCARD");
        vCardText.AppendLine("VERSION:3.0");
        vCardText.AppendLine($"FN:{fullName}");
        vCardText.AppendLine($"TEL:{phone}");
        vCardText.AppendLine($"ORG:{organization}");
        vCardText.AppendLine($"TITLE:{position}");
        vCardText.AppendLine("END:VCARD");
        
        var vCardBytes = Encoding.UTF8.GetBytes(vCardText.ToString());
        
        return File(vCardBytes, "text/vcard", "contact.vcf", true);
    }

    private bool ClientExists(long id)
    {
        return _context.Clients.Any(e => e.Id == id);
    }
}