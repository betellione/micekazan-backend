#nullable disable

using Micekazan.PrintDispatcher.Models;
using Microsoft.EntityFrameworkCore;

namespace Micekazan.PrintDispatcher.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TicketToPrint> TicketsToPrint { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketToPrint>(entity =>
        {
            entity.ToTable("TicketToPrint");
            entity.HasKey(x => x.Barcode).HasName("TicketToPrint_pk");

            entity.Property(x => x.Barcode).HasColumnName("Barcode");

            entity.Property(x => x.Url).HasMaxLength(256).HasColumnName("Url");
        });
    }
}