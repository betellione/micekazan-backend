using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class PhoneCalling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConfirmationPhoneCall",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserPhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ConfirmationToken = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: false),
                    ConfirmationPhoneCode = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UserConfirmationPhoneCall_pk", x => new { x.UserId, x.Timestamp });
                    table.ForeignKey(
                        name: "UserConfirmationPhoneCall_User_UserId_fk",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConfirmationPhoneCall");
        }
    }
}
