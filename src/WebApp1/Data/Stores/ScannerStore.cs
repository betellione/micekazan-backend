using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.Data.Stores;

public class ScannerStore : IScannerStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly UserManager<User> _userManager;

    public ScannerStore(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<User> userManager)
    {
        _contextFactory = contextFactory;
        _userManager = userManager;
    }

    public async Task<EventScanner?> FindScannerById(Guid userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var scanner = await context.EventScanners.FirstOrDefaultAsync(x => x.ScannerId == userId);
        return scanner;
    }

    public async Task<bool> SetClaimsForScanner(Guid userId, bool isAutomate)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var claimIsAutomate = (await _userManager.GetClaimsAsync(user!)).Any(x => x.Value == "Automate");

        if (isAutomate == claimIsAutomate) return false;
        if (isAutomate && !claimIsAutomate) await _userManager.AddClaimAsync(user!, new Claim(ClaimTypes.Actor, "Automate"));
        else await _userManager.RemoveClaimAsync(user!, new Claim(ClaimTypes.Actor, "Automate"));
        return true;
    }
}