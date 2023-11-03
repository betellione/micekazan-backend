using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApp1.Models;

namespace WebApp1.Data;

public static class Seeding
{
    public static async Task SeedAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();

        var exists = await userStore.FindByNameAsync("ADMIN", default);
        if (exists is null)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var email = configuration["Admin:Email"]!;
            var password = configuration["Admin:Password"]!;
            await CreateAdmin(userManager, email, password);
        }
    }

    private static async Task CreateAdmin(UserManager<User> userManager, string email, string password)
    {
        const string username = "admin";

        var user = new User();

        await userManager.SetUserNameAsync(user, username);
        await userManager.SetEmailAsync(user, email);
        _ = await userManager.CreateAsync(user, password);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await userManager.ConfirmEmailAsync(user, token);

        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email!));
        await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Admin"));
    }
}