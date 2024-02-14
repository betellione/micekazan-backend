using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class TicketPdfTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client",
                column: "Email");

            migrationBuilder.CreateTable(
                name: "TicketPdfTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TextColor = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: false),
                    IsHorizontal = table.Column<bool>(type: "boolean", nullable: false),
                    HasName = table.Column<bool>(type: "boolean", nullable: false),
                    HasSurname = table.Column<bool>(type: "boolean", nullable: false),
                    HasQrCode = table.Column<bool>(type: "boolean", nullable: false),
                    LogoUri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BackgroundUri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    OrganizerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TicketPdfTemplate_pk", x => x.Id);
                    table.ForeignKey(
                        name: "TicketPdfTemplate_User_OrganizerId_fk",
                        column: x => x.OrganizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketPdfTemplate_OrganizerId",
                table: "TicketPdfTemplate",
                column: "OrganizerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketPdfTemplate");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client");
        }
    }
}
