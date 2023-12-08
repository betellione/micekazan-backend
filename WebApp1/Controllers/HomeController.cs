using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data.Stores;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.ViewModels.Event;
using WebApp1.ViewModels.Home;

namespace WebApp1.Controllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IScreenStore _screenStore;
    private readonly IScannerStore _scannerStore;

    public HomeController(IEventService eventService, SignInManager<User> signInManager, UserManager<User> userManager, IScreenStore screenStore, IScannerStore scannerStore)
    {
        _eventService = eventService;
        _signInManager = signInManager;
        _userManager = userManager;
        _screenStore = screenStore;
        _scannerStore = scannerStore;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account");

        if (User.Claims.Any(x => x is { Type: ClaimTypes.Actor, Value: "Automate" })) return RedirectToAction("Terminal", "Home");
        
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
    
    [HttpGet]
    public async Task<IActionResult> Terminal()
    {
        var scanner = await _scannerStore.FindScannerById(new Guid(_userManager.GetUserId(User)!));
        if (scanner is null) return NotFound();
        var vm = await _screenStore.GetScreenByType(scanner.EventId, ScreenTypes.Waiting);
        if (vm is null) return View(new ScreenViewModel());
        return View(new ScreenViewModel
        {
            MainText = vm.WelcomeText,
            Description = vm.Description,
            BackgroundColor = vm.TextColor,
        });
    }
}