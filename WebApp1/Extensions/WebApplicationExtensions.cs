using Serilog;
using WebApp1.Data;

namespace WebApp1.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }

    public static async Task<IApplicationBuilder> SetupDatabaseAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            await Seeding.SeedAdmin(scope.ServiceProvider);

            return app;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot setup database: {e.Message}");
            throw;
        }
    }
}