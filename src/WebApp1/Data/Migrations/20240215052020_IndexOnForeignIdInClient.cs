using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexOnForeignIdInClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client");

            migrationBuilder.RenameIndex(
                name: "IX_Client_Email",
                table: "Client",
                newName: "Client_Email_ix");

            migrationBuilder.CreateIndex(
                name: "Client_ForeignId_ix",
                table: "Client",
                column: "ForeignId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Client_ForeignId_ix",
                table: "Client");

            migrationBuilder.RenameIndex(
                name: "Client_Email_ix",
                table: "Client",
                newName: "IX_Client_Email");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Client_Email",
                table: "Client",
                column: "Email");
        }
    }
}
