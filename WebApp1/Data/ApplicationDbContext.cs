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
    public DbSet<EventScanner> EventScanners { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TokenUpdate> TokenUpdates { get; set; }
    public DbSet<TicketToPrint> TicketsToPrint { get; set; }
    public DbSet<TicketPdfTemplate> TicketPdfTemplate { get; set; }
    public DbSet<Screen> Screen { get; set; }
    public DbSet<InfoToShow> InfoToShow { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(64).HasColumnName("Name");
            entity.Property(x => x.Surname).HasMaxLength(64).HasColumnName("Surname");
            entity.Property(x => x.Patronymic).HasMaxLength(64).HasColumnName("Patronymic");
            entity.Property(x => x.City).HasMaxLength(32).HasColumnName("City");
            entity.Property(x => x.Activity).HasColumnName("Activity");
        });

        modelBuilder.Entity<InfoToShow>(entity =>
        {
            entity.ToTable("InfoToShow");

            entity.HasKey(x => x.Id).HasName("InfoToShow_pk");
            entity.HasAlternateKey(x => x.Token).HasName("InfoToShow_pk2");

            entity.Property(x => x.Email).HasMaxLength(64).HasColumnName("Email");
            entity.Property(x => x.Phone).HasMaxLength(20).HasColumnName("Phone");
            entity.Property(x => x.Barcode).HasMaxLength(32).HasColumnName("Barcode");
            entity.Property(x => x.Token).HasMaxLength(22).HasColumnName("Token");
            entity.Property(x => x.ClientName).HasMaxLength(64).HasColumnName("ClientName");
            entity.Property(x => x.ClientSurname).HasMaxLength(64).HasColumnName("ClientSurname");
            entity.Property(x => x.ClientMiddleName).HasMaxLength(64).HasColumnName("ClientMiddleName");
            entity.Property(x => x.OrganizationName).HasMaxLength(64).HasColumnName("OrganizationName");
            entity.Property(x => x.WorkPosition).HasMaxLength(64).HasColumnName("WorkPosition");
        });

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
            entity.Property(x => x.ForeignShowIds).HasDefaultValueSql("'{}'").HasColumnName("ForeignShowIds");

            entity.HasOne(x => x.Creator).WithMany(x => x.EventsCreated)
                .HasForeignKey(x => x.CreatorId)
                .HasConstraintName("Event_User_CreatorId_fk");
        });

        modelBuilder.Entity<EventScanner>(entity =>
        {
            entity.ToTable("EventCollector");

            entity.HasKey(x => new { CollectorId = x.ScannerId, x.EventId, }).HasName("EventCollector_pk");

            entity.Property(x => x.ScannerId).HasColumnName("CollectorId");
            entity.Property(x => x.EventId).HasColumnName("EventId");
            entity.Property(x => x.TicketPdfTemplateId).HasColumnName("TicketPdfTemplateId");
            entity.Property(x => x.PrintingToken).HasMaxLength(256).HasColumnName("PrintingToken");

            entity.HasOne(x => x.Scanner).WithMany(x => x.EventsToCollect)
                .HasForeignKey(x => x.ScannerId)
                .HasConstraintName("EventCollector_User_CollectorId_fk");

            entity.HasOne(x => x.Event).WithMany(x => x.Collectors)
                .HasForeignKey(x => x.EventId)
                .HasConstraintName("EventCollector_Event_EventId_fk");

            entity.HasOne(x => x.TicketPdfTemplate).WithMany(x => x.ScannersWithTemplate)
                .HasForeignKey(x => x.TicketPdfTemplateId)
                .HasConstraintName("EventCollector_TicketPdfTemplate_TicketPdfTemplateId_fk");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasKey(x => x.Id).HasName("Ticket_pk");

            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.Barcode).HasMaxLength(16).HasColumnName("Barcode");
            entity.Property(x => x.PassedAt).HasColumnName("PassedAt");
            entity.Property(x => x.EventId).HasColumnName("EventId");

            entity.HasOne(x => x.Client).WithMany(x => x.Tickets)
                .HasForeignKey(x => x.ClientId)
                .HasConstraintName("Ticket_Client_ClientId_fk");
            entity.HasOne(x => x.Event).WithMany(x => x.Tickets)
                .HasForeignKey(x => x.EventId)
                .HasConstraintName("Ticket_Event_EventId_fk");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Client");

            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.ForeignId).HasColumnName("ForeignId");
            entity.Property(x => x.Name).HasMaxLength(64).HasColumnName("Name");
            entity.Property(x => x.Surname).HasMaxLength(64).HasColumnName("Surname");
            entity.Property(x => x.Patronymic).HasMaxLength(64).HasColumnName("Patronymic");
            entity.Property(x => x.Email).HasMaxLength(64).HasColumnName("Email");
            entity.Property(x => x.PhoneNumber).HasMaxLength(64).HasColumnName("PhoneNumber");

            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasAlternateKey(x => x.Email);
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

        modelBuilder.Entity<TicketPdfTemplate>(entity =>
        {
            entity.ToTable("TicketPdfTemplate");
            entity.HasKey(x => x.Id).HasName("TicketPdfTemplate_pk");

            entity.Property(x => x.TemplateName).HasMaxLength(16).HasColumnName("TemplateName");
            entity.Property(x => x.TextColor).HasMaxLength(7).IsFixedLength().HasColumnName("TextColor").HasDefaultValue("#000000");

            entity.Property(x => x.IsHorizontal).HasColumnName("IsHorizontal").HasDefaultValue(true);
            entity.Property(x => x.HasName).HasColumnName("HasName").HasDefaultValue(true);
            entity.Property(x => x.HasSurname).HasColumnName("HasSurname").HasDefaultValue(true);
            entity.Property(x => x.HasQrCode).HasColumnName("HasQrCode").HasDefaultValue(true);

            entity.Property(x => x.LogoUri).HasMaxLength(2048).HasColumnName("LogoUri");
            entity.Property(x => x.BackgroundUri).HasMaxLength(2048).HasColumnName("BackgroundUri");

            entity.HasOne(x => x.Organizer).WithMany(x => x.TicketPdfTemplates)
                .HasForeignKey(x => x.OrganizerId)
                .HasConstraintName("TicketPdfTemplate_User_OrganizerId_fk");
        });

        modelBuilder.Entity<Screen>(entity =>
        {
            entity.ToTable("Screen");
            entity.HasKey(x => x.Id).HasName("Screen_pk");

            entity.Property(x => x.Type).HasColumnName("Type");
            entity.Property(x => x.WelcomeText).HasMaxLength(32).HasColumnName("WelcomeText");
            entity.Property(x => x.Description).HasMaxLength(256).HasColumnName("Description");
            entity.Property(x => x.TextColor).HasMaxLength(7).IsFixedLength().HasColumnName("TextColor").HasDefaultValue("#000000");

            entity.Property(x => x.LogoUri).HasMaxLength(2048).HasColumnName("LogoUri");
            entity.Property(x => x.BackgroundUri).HasMaxLength(2048).HasColumnName("BackgroundUri");

            entity.HasOne(x => x.Event).WithMany(x => x.Screens)
                .HasForeignKey(x => x.EventId)
                .HasConstraintName("Screen_Event_EventId_fk");
        });
    }
}