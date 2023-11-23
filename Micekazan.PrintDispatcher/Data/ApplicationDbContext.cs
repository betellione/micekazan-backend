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

            entity.HasKey(x => x.Id).HasName("TicketToPrint_pk");

            entity.Property(x => x.Barcode).HasMaxLength(16).HasColumnName("Barcode");
            entity.Property(x => x.FilePath).HasMaxLength(256).HasColumnName("FilePath");
            entity.Property(x => x.PrintingToken).HasMaxLength(256).HasColumnName("PrintingToken");
            entity.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(x => x.DeletedAt).HasColumnName("DeletedAt");
            entity.Property(x => x.PrintedAt).HasColumnName("PrintedAt");
        });
    }
}