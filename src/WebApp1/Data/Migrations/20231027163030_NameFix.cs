using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class NameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenUpdate_AspNetUsers_TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate");

            migrationBuilder.DropIndex(
                name: "IX_TokenUpdate_TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate");

            migrationBuilder.DropColumn(
                name: "TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate");

            migrationBuilder.CreateIndex(
                name: "IX_TokenUpdate_CreatorId",
                table: "TokenUpdate",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate");

            migrationBuilder.DropIndex(
                name: "IX_TokenUpdate_CreatorId",
                table: "TokenUpdate");

            migrationBuilder.AddColumn<Guid>(
                name: "TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TokenUpdate_TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate",
                column: "TokenUpdate_User_CreatorId_fk");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenUpdate_AspNetUsers_TokenUpdate_User_CreatorId_fk",
                table: "TokenUpdate",
                column: "TokenUpdate_User_CreatorId_fk",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
