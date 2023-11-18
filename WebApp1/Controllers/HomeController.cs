using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Models;

namespace WebApp1.Controllers;

public class HomeController(SignInManager<User> signInManager) : Controller
{
    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (!signInManager.IsSignedIn(User)) return RedirectToAction("Login", "Account");
        return User.Claims.First(x => x.Type == ClaimTypes.Role).Value switch
        {
            "Organizer" => RedirectToAction("Index", "Event"),
            "Admin" => RedirectToAction("Index", "User"),
            "Scanner" => View(),
            _ => signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account"),
        };
    }
}