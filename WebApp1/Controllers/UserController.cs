using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Mapping;
using WebApp1.Models;
using WebApp1.ViewModels;

namespace WebApp1.Controllers;

[Authorize]
public class UserController(ApplicationDbContext context, IUserStore<User> userStore, UserManager<User> userManager)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await userManager.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, "Organizer"));
        return View(users.Select(x => x.UserMapping()));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null) return BadRequest();

        var user = await context.Users.FirstOrDefaultAsync(m => m.Id == id);
        if (user is null) return NotFound();

        return View(user);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new User
        {
            Email = vm.Email,
            NormalizedEmail = userManager.NormalizeEmail(vm.Email),
            EmailConfirmed = true,
        };

        await userStore.SetUserNameAsync(user, vm.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(vm);
        }

        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, vm.Email));
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Scanner"));
        return RedirectToAction("Index", "Event");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null) return BadRequest();

        var user = await context.Users.FindAsync(id);
        if (user is null) return NotFound();

        return View(user.UserMapping());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        [Bind(
            "CreatedAt,ExpiresAt,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]
        User user)
    {
        if (id != user.Id) return NotFound();
        if (!ModelState.IsValid) return View(user.UserMapping());

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(user.Id)) return NotFound();
            throw;
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) return BadRequest();

        var user = await context.Users.FirstOrDefaultAsync(m => m.Id == id);
        if (user is null) return NotFound();

        return View(user);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null) return NotFound();

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    private Task<bool> UserExists(Guid id)
    {
        return context.Users.AnyAsync(e => e.Id == id);
    }
}