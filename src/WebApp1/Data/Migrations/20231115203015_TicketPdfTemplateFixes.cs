using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class TicketPdfTemplateFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TextColor",
                table: "TicketPdfTemplate",
                type: "character(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: false,
                defaultValue: "#000000",
                oldClrType: typeof(string),
                oldType: "character(7)",
                oldFixedLength: true,
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<bool>(
                name: "IsHorizontal",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HasSurname",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HasQrCode",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HasName",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TextColor",
                table: "TicketPdfTemplate",
                type: "character(7)",
                fixedLength: true,
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(7)",
                oldFixedLength: true,
                oldMaxLength: 7,
                oldDefaultValue: "#000000");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHorizontal",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasSurname",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasQrCode",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasName",
                table: "TicketPdfTemplate",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);
        }
    }
}
