#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<CreatorToken> CreatorTokens { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventCollector> EventCollectors { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TokenUpdate> TokenUpdates { get; set; }
    public DbSet<TicketToPrint> TicketsToPrint { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CreatorToken>(entity =>
        {
            entity.ToTable("CreatorToken");

            entity.HasKey(x => x.CreatorId).HasName("CreatorToken_pk");

            entity.HasIndex(x => x.Token).IsUnique();

            entity.Property(x => x.CreatorId).HasColumnName("CreatorId");
            entity.Property(x => x.Token).HasMaxLength(128).HasColumnName("Token");

            entity.HasOne(x => x.Creator).WithMany(x => x.Tokens)
                .HasForeignKey(x => x.CreatorId)
                .HasConstraintName("CreatorToken_User_CreatorId_fk");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.HasKey(x => x.Id).HasName("Event_pk");

            entity.Property(x => x.Id).ValueGeneratedNever().HasColumnName("Id");
            entity.Property(x => x.Name).HasMaxLength(256).HasColumnName("Name");
            entity.Property(x => x.City).HasMaxLength(128).HasColumnName("City");
            entity.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(x => x.StartedAt).HasColumnName("StartedAt");
            entity.Property(x => x.FinishedAt).HasColumnName("FinishedAt");
            entity.Property(x => x.CreatorId).HasColumnName("CreatorId");

            entity.HasOne(x => x.Creator).WithMany(x => x.EventsCreated)
                .HasForeignKey(x => x.CreatorId)
                .HasConstraintName("Event_User_CreatorId_fk");
        });

        modelBuilder.Entity<EventCollector>(entity =>
        {
            entity.ToTable("EventCollector");

            entity.HasKey(x => new { x.CollectorId, x.EventId }).HasName("EventCollector_pk");

            entity.Property(x => x.CollectorId).HasColumnName("CollectorId");
            entity.Property(x => x.EventId).HasColumnName("EventId");

            entity.HasOne(x => x.Collector).WithMany(x => x.EventsToCollect)
                .HasForeignKey(x => x.CollectorId)
                .HasConstraintName("EventCollector_User_CollectorId_fk");

            entity.HasOne(x => x.Event).WithMany(x => x.Collectors)
                .HasForeignKey(x => x.EventId)
                .HasConstraintName("EventCollector_Event_EventId_fk");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasKey(x => x.Id).HasName("Ticket_pk");

            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.Barcode).HasMaxLength(16).HasColumnName("Barcode");
            entity.Property(x => x.EventId).HasColumnName("EventId");

            entity.HasOne(x => x.Event).WithMany(x => x.Tickets)
                .HasForeignKey(x => x.EventId)
                .HasConstraintName("Ticket_Event_EventId_fk");
        });

        modelBuilder.Entity<TokenUpdate>(entity =>
        {
            entity.ToTable("TokenUpdate");

            entity.HasKey(x => x.Id).HasName("TokenUpdate_pk");

            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.CreatorId).HasColumnName("CreatorId");
            entity.Property(x => x.Token).HasMaxLength(128).HasColumnName("Token");
            entity.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");

            entity.HasOne(x => x.Creator).WithMany(x => x.TokenUpdates)
                .HasForeignKey(x => x.CreatorId)
                .HasConstraintName("TokenUpdate_User_CreatorId_fk");
        });

        modelBuilder.Entity<TicketToPrint>(entity =>
        {
            entity.ToTable("TicketToPrint");
            entity.HasKey(x => x.Barcode).HasName("TicketToPrint_pk");

            entity.Property(x => x.Barcode).HasColumnName("Barcode");

            entity.Property(x => x.Url).HasMaxLength(256).HasColumnName("Url");
        });
    }
}