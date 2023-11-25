using Micekazan.PrintDispatcher.Data;
using Microsoft.EntityFrameworkCore;

namespace Micekazan.PrintDispatcher.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<IApplicationBuilder> SetupDatabaseAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            return app;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot setup the database: {e.Message}");
            throw;
        }
    }
}