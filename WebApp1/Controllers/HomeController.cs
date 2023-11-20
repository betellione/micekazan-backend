using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.ViewModels.Home;

namespace WebApp1.Controllers;

public class HomeController(ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager) : Controller
{
    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (!signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account");
        
        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(new IndexViewModel
            {
                AllTickets = context.Tickets
                    .Count(t => t.Event.Collectors.Select(x => x.CollectorId).Contains(new Guid(userManager.GetUserId(User)!))),
                ScannedTickets = context.Tickets.Where(x => x.PassedAt != null)
                    .Count(t => t.Event.Collectors.Select(x => x.CollectorId).Contains(new Guid(userManager.GetUserId(User)!)))
            }),
            _ => signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account"),
        };
    }
}