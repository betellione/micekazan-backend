using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApp1.Models;

namespace WebApp1.Data;

public static class Seeding
{
    public static async Task SeedAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var email = configuration["Admin:Email"]!;

        var exists = await userManager.FindByEmailAsync(email);
        if (exists is null)
        {
            var password = configuration["Admin:Password"]!;
            await CreateAdmin(userManager, email, password);
        }
    }

    private static async Task CreateAdmin(UserManager<User> userManager, string email, string password)
    {
        var user = new User();

        await userManager.SetUserNameAsync(user, email);
        await userManager.SetEmailAsync(user, email);
        _ = await userManager.CreateAsync(user, password);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await userManager.ConfirmEmailAsync(user, token);

        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email!));
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Admin"));
    }
}