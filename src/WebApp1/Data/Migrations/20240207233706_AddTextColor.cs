using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTextColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "Screen",
                type: "character(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: false,
                defaultValue: "#FFFFFF");

            migrationBuilder.AddColumn<int>(
                name: "TextSize",
                table: "Screen",
                type: "integer",
                nullable: false,
                defaultValue: 70);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "Screen");

            migrationBuilder.DropColumn(
                name: "TextSize",
                table: "Screen");
        }
    }
}
