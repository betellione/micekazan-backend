using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForeignShowsIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<long>>(
                name: "ForeignShowIds",
                table: "Event",
                type: "bigint[]",
                nullable: false,
                defaultValueSql: "'{}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForeignShowIds",
                table: "Event");
        }
    }
}
