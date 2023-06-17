using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipping.DAL.Migrations
{
    /// <inheritdoc />
    public partial class sgippin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Governorates");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Groups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "Governorates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
