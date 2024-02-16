using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Data.Stores;
using WebApp1.Enums;
using WebApp1.Models;
using WebApp1.Services.EventService;
using WebApp1.Services.PrintService;
using WebApp1.Services.TicketService;
using WebApp1.ViewModels.Event;
using WebApp1.ViewModels.Home;

namespace WebApp1.Controllers;

public class HomeController : Controller
{
    private readonly IEventService _eventService;
    private readonly IPrintService _printService;
    private readonly IScannerStore _scannerStore;
    private readonly IScreenStore _screenStore;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ITicketService _ticketService;

    public HomeController(IEventService eventService, SignInManager<User> signInManager, UserManager<User> userManager,
        IScreenStore screenStore, IScannerStore scannerStore, IPrintService printService, ITicketService ticketService)
    {
        _eventService = eventService;
        _signInManager = signInManager;
        _userManager = userManager;
        _screenStore = screenStore;
        _scannerStore = scannerStore;
        _printService = printService;
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        if (!_signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account", new { returnUrl, });
        if (User.Claims.Any(x => x is { Type: ClaimTypes.Actor, Value: "Automate", })) return RedirectToAction("Terminal", "Home");

        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(new IndexViewModel
            {
                AllTickets = await _eventService.GetAllTicketsNumber(new Guid(_userManager.GetUserId(User)!)),
                ScannedTickets = await _eventService.GetScannedTicketsNumber(new Guid(_userManager.GetUserId(User)!)),
            }),
            _ => _signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account", new { returnUrl, }),
        };
    }

    [HttpGet]
    public async Task<IActionResult> Terminal(ScreenTypes screenType = ScreenTypes.Waiting)
    {
        var scanner = await _scannerStore.FindScannerById(new Guid(_userManager.GetUserId(User)!));
        if (scanner is null) return NotFound();
        var screen = await _screenStore.GetScreenByType(scanner.EventId, screenType);
        if (screen is null) return View(new ScreenViewModel());
        return View(new ScreenViewModel
        {
            MainText = screen.WelcomeText,
            Description = screen.Description,
            TextColor = screen.TextColor,
            TextSize = screen.TextSize,
            BackgroundColor = screen.BackgroundColor,
            LogoPath = screen.LogoUri,
            BackgroundPath = screen.BackgroundUri,
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> TerminalSecond(ScreenTypes screenType = ScreenTypes.Waiting)
    {
        var scanner = await _scannerStore.FindScannerById(new Guid(_userManager.GetUserId(User)!));
        if (scanner is null) return NotFound();
        var screen = await _screenStore.GetScreenByType(scanner.EventId, screenType);
        if (screen is null) return View(new ScreenViewModel());
        return View(new ScreenViewModel
        {
            MainText = screen.WelcomeText,
            Description = screen.Description,
            TextColor = screen.TextColor,
            TextSize = screen.TextSize,
            BackgroundColor = screen.BackgroundColor,
            LogoPath = screen.LogoUri,
            BackgroundPath = screen.BackgroundUri,
        });
    }

    [HttpPost]
    public async Task<IActionResult> PrintTerminal(string code)
    {
        var userId = new Guid(_userManager.GetUserId(User)!);

        var printingToken = await _scannerStore.GetScannerPrintingToken(userId);
        if (printingToken is null) return BadRequest();

        await using var pdf = await _ticketService.GetTicketPdf(userId, code);
        if (pdf is null) return BadRequest();

        if (!await _ticketService.SetPassTime(code))
            return BadRequest();

        await _printService.AddTicketToPrintQueue(pdf, printingToken, code);

        return Ok();
    }
    
}