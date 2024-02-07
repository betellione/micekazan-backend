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
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Client",
                table: "Client");

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

            migrationBuilder.AddPrimaryKey(
                name: "Client_pk",
                table: "Client",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "Client_pk",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "Screen");

            migrationBuilder.DropColumn(
                name: "TextSize",
                table: "Screen");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Client",
                table: "Client",
                column: "Id");
        }
    }
}
