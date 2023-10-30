using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Models;

namespace WebApp1.Controllers;

public class HomeController(SignInManager<User> signInManager) : Controller
{
    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        return signInManager.IsSignedIn(User) ? View() : RedirectToAction("Login", "Account");
    }
}