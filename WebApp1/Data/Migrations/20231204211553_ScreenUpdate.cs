using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class ScreenUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Screen_EventId",
                table: "Screen");

            migrationBuilder.CreateIndex(
                name: "IX_Screen_EventId_Type",
                table: "Screen",
                columns: new[] { "EventId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Screen_EventId_Type",
                table: "Screen");

            migrationBuilder.CreateIndex(
                name: "IX_Screen_EventId",
                table: "Screen",
                column: "EventId");
        }
    }
}
