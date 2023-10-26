using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApp1.Areas.Identity.Pages.Account;
using WebApp1.Models;

namespace WebApp1.Pages;

public class IndexModel : PageModel
{
    
    private readonly SignInManager<User> _signInManager;

    public IndexModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public IActionResult  OnGetAsync()
    {
        return _signInManager.IsSignedIn(User) ? Page() :  LocalRedirect("~/Identity/Account/Login");
    }
}