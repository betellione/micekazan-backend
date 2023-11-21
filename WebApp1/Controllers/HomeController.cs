using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.ViewModels.Home;

namespace WebApp1.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public HomeController(ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account");

        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(new IndexViewModel
            {
                AllTickets = _context.Tickets
                    .Count(t => t.Event.Collectors.Select(x => x.ScannerId).Contains(new Guid(_userManager.GetUserId(User)!))),
                ScannedTickets = _context.Tickets.Where(x => x.PassedAt != null)
                    .Count(t => t.Event.Collectors.Select(x => x.ScannerId).Contains(new Guid(_userManager.GetUserId(User)!))),
            }),
            _ => _signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account"),
        };
    }
}