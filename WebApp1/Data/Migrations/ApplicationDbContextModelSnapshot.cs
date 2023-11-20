﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApp1.Data;

#nullable disable

namespace WebApp1.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.Client", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Email");

                    b.Property<long>("ForeignId")
                        .HasColumnType("bigint")
                        .HasColumnName("ForeignId");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Name");

                    b.Property<string>("Patronymic")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Patronymic");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("PhoneNumber");

                    b.Property<string>("Surname")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Surname");

                    b.HasKey("Id");

                    b.HasAlternateKey("Email");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Client", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.CreatorToken", b =>
                {
                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid")
                        .HasColumnName("CreatorId");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("Token");

                    b.HasKey("CreatorId")
                        .HasName("CreatorToken_pk");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.ToTable("CreatorToken", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.Event", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("City");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid")
                        .HasColumnName("CreatorId");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("FinishedAt");

                    b.Property<long[]>("ForeignShowIds")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint[]")
                        .HasColumnName("ForeignShowIds")
                        .HasDefaultValueSql("'{}'");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("Name");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("StartedAt");

                    b.HasKey("Id")
                        .HasName("Event_pk");

                    b.HasIndex("CreatorId");

                    b.ToTable("Event", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.EventCollector", b =>
                {
                    b.Property<Guid>("CollectorId")
                        .HasColumnType("uuid")
                        .HasColumnName("CollectorId");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint")
                        .HasColumnName("EventId");

                    b.HasKey("CollectorId", "EventId")
                        .HasName("EventCollector_pk");

                    b.HasIndex("EventId");

                    b.ToTable("EventCollector", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.Screen", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("BackgroundUri")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("BackgroundUri");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("Description");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("LogoUri")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("LogoUri");

                    b.Property<string>("TextColor")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(7)
                        .HasColumnType("character(7)")
                        .HasDefaultValue("#000000")
                        .HasColumnName("TextColor")
                        .IsFixedLength();

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("Type");

                    b.Property<string>("WelcomeText")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("WelcomeText");

                    b.HasKey("Id")
                        .HasName("Screen_pk");

                    b.HasIndex("EventId");

                    b.ToTable("Screen", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.Ticket", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("Barcode");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint")
                        .HasColumnName("EventId");

                    b.Property<DateTime?>("PassedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("PassedAt");

                    b.HasKey("Id")
                        .HasName("Ticket_pk");

                    b.HasIndex("ClientId");

                    b.HasIndex("EventId");

                    b.ToTable("Ticket", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.TicketPdfTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("BackgroundUri")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("BackgroundUri");

                    b.Property<bool>("HasName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("HasName");

                    b.Property<bool>("HasQrCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("HasQrCode");

                    b.Property<bool>("HasSurname")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("HasSurname");

                    b.Property<bool>("IsHorizontal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("IsHorizontal");

                    b.Property<string>("LogoUri")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("LogoUri");

                    b.Property<Guid>("OrganizerId")
                        .HasColumnType("uuid");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("TemplateName");

                    b.Property<string>("TextColor")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(7)
                        .HasColumnType("character(7)")
                        .HasDefaultValue("#000000")
                        .HasColumnName("TextColor")
                        .IsFixedLength();

                    b.HasKey("Id")
                        .HasName("TicketPdfTemplate_pk");

                    b.HasIndex("OrganizerId");

                    b.ToTable("TicketPdfTemplate", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.TicketToPrint", b =>
                {
                    b.Property<string>("Barcode")
                        .HasColumnType("text")
                        .HasColumnName("Barcode");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("Url");

                    b.HasKey("Barcode")
                        .HasName("TicketToPrint_pk");

                    b.ToTable("TicketToPrint", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.TokenUpdate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid")
                        .HasColumnName("CreatorId");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("Token");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id")
                        .HasName("TokenUpdate_pk");

                    b.HasIndex("CreatorId");

                    b.ToTable("TokenUpdate", (string)null);
                });

            modelBuilder.Entity("WebApp1.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<int?>("Activity")
                        .HasColumnType("integer")
                        .HasColumnName("Activity");

                    b.Property<string>("City")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Patronymic");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("PrintingToken")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("PrintingToken");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("Surname");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("WebApp1.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("WebApp1.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApp1.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("WebApp1.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApp1.Models.CreatorToken", b =>
                {
                    b.HasOne("WebApp1.Models.User", "Creator")
                        .WithMany("Tokens")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("CreatorToken_User_CreatorId_fk");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("WebApp1.Models.Event", b =>
                {
                    b.HasOne("WebApp1.Models.User", "Creator")
                        .WithMany("EventsCreated")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Event_User_CreatorId_fk");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("WebApp1.Models.EventCollector", b =>
                {
                    b.HasOne("WebApp1.Models.User", "Collector")
                        .WithMany("EventsToCollect")
                        .HasForeignKey("CollectorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("EventCollector_User_CollectorId_fk");

                    b.HasOne("WebApp1.Models.Event", "Event")
                        .WithMany("Collectors")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("EventCollector_Event_EventId_fk");

                    b.Navigation("Collector");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("WebApp1.Models.Screen", b =>
                {
                    b.HasOne("WebApp1.Models.Event", "Event")
                        .WithMany("Screens")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Screen_Event_EventId_fk");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("WebApp1.Models.Ticket", b =>
                {
                    b.HasOne("WebApp1.Models.Client", "Client")
                        .WithMany("Tickets")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Ticket_Client_ClientId_fk");

                    b.HasOne("WebApp1.Models.Event", "Event")
                        .WithMany("Tickets")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Ticket_Event_EventId_fk");

                    b.Navigation("Client");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("WebApp1.Models.TicketPdfTemplate", b =>
                {
                    b.HasOne("WebApp1.Models.User", "Organizer")
                        .WithMany("TicketPdfTemplates")
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("TicketPdfTemplate_User_OrganizerId_fk");

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("WebApp1.Models.TokenUpdate", b =>
                {
                    b.HasOne("WebApp1.Models.User", "Creator")
                        .WithMany("TokenUpdates")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("TokenUpdate_User_CreatorId_fk");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("WebApp1.Models.Client", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("WebApp1.Models.Event", b =>
                {
                    b.Navigation("Collectors");

                    b.Navigation("Screens");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("WebApp1.Models.User", b =>
                {
                    b.Navigation("EventsCreated");

                    b.Navigation("EventsToCollect");

                    b.Navigation("TicketPdfTemplates");

                    b.Navigation("TokenUpdates");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
