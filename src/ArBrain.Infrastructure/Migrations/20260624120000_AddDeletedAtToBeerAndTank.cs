using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArBrain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToBeerAndTank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "tanks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "beers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE beers SET \"UpdatedAt\" = \"CreatedAt\" WHERE \"UpdatedAt\" IS NULL");

            migrationBuilder.Sql(
                "UPDATE tanks SET \"UpdatedAt\" = \"CreatedAt\" WHERE \"UpdatedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "tanks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "beers");
        }
    }
}
