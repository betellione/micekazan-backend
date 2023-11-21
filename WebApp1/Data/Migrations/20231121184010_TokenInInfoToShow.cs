using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class TokenInInfoToShow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "InfoToShow",
                type: "character varying(22)",
                maxLength: 22,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "InfoToShow_pk2",
                table: "InfoToShow",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "InfoToShow_pk2",
                table: "InfoToShow");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "InfoToShow");
        }
    }
}
