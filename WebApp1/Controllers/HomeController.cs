using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.ViewModels.Home;

namespace WebApp1.Controllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public HomeController(IEventService eventService, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _eventService = eventService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account");

        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(new IndexViewModel
            {
                AllTickets = await _eventService.GetAllTicketsNumber(new Guid(_userManager.GetUserId(User)!)),
                ScannedTickets = await _eventService.GetScannedTicketsNumber(new Guid(_userManager.GetUserId(User)!)),
            }),
            _ => _signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account"),
        };
    }
}