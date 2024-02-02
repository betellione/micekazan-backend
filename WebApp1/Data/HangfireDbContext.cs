using Microsoft.EntityFrameworkCore;

namespace WebApp1.Data;

public class HangfireDbContext : DbContext
{
    public HangfireDbContext(string connectionString) : base(GetOptions(connectionString))
    {
    }

    public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options)
    {
    }

    private static DbContextOptions<HangfireDbContext> GetOptions(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<HangfireDbContext>();
        builder.UseNpgsql(connectionString);
        return builder.Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}