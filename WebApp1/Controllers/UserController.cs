using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;
using WebApp1.ViewModels;

namespace WebApp1.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;

    public UserController(ApplicationDbContext context, IUserStore<User> userStore, UserManager<User> userManager)
    {
        _context = context;
        _emailStore = (IUserEmailStore<User>)_userStore!;
        _userStore = userStore;
        _userManager = userManager;
    }

    // GET: User
    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }

    // GET: User/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null) return NotFound();

        return View(user);
    }

    // GET: User/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = new User
        {
            Email = vm.Email,
            NormalizedEmail = _userManager.NormalizeEmail(vm.Email),
            EmailConfirmed = true
        };

        await _userStore.SetUserNameAsync(user, vm.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, vm.Email));
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Organizer"));
        return RedirectToAction("Index");
    }

    // GET: User/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    // POST: User/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,
        [Bind(
            "CreatedAt,ExpiresAt,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]
        User user)
    {
        if (id != user.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: User/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null) _context.Users.Remove(user);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(Guid id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}