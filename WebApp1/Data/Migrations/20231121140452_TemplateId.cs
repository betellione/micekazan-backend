using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class TemplateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrintingToken",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PrintingToken",
                table: "EventCollector",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketPdfTemplateId",
                table: "EventCollector",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCollector_TicketPdfTemplateId",
                table: "EventCollector",
                column: "TicketPdfTemplateId");

            migrationBuilder.AddForeignKey(
                name: "EventCollector_TicketPdfTemplate_TicketPdfTemplateId_fk",
                table: "EventCollector",
                column: "TicketPdfTemplateId",
                principalTable: "TicketPdfTemplate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "EventCollector_TicketPdfTemplate_TicketPdfTemplateId_fk",
                table: "EventCollector");

            migrationBuilder.DropIndex(
                name: "IX_EventCollector_TicketPdfTemplateId",
                table: "EventCollector");

            migrationBuilder.DropColumn(
                name: "PrintingToken",
                table: "EventCollector");

            migrationBuilder.DropColumn(
                name: "TicketPdfTemplateId",
                table: "EventCollector");

            migrationBuilder.AddColumn<string>(
                name: "PrintingToken",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
