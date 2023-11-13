using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Models;

namespace WebApp1.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<User> _signInManager;

    public HomeController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (!_signInManager.IsSignedIn(User))
            return RedirectToAction("Login", "Account");
        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(),
            _ => _signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account")
        };
    }
}