using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexesInTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ForeignId",
                table: "Ticket",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "Ticket_Barcode_ix",
                table: "Ticket",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "Ticket_ForeignId_ix",
                table: "Ticket",
                column: "ForeignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Ticket_Barcode_ix",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "Ticket_ForeignId_ix",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ForeignId",
                table: "Ticket");
        }
    }
}
