using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Backend.Migrations
{
    /// <inheritdoc />
    public partial class LastActivityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActive",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActive",
                schema: "dbo",
                table: "Users");
        }
    }
}
