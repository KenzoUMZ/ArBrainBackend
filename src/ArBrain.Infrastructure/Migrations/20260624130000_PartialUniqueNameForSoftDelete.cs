using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArBrain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PartialUniqueNameForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_beers_Name",
                table: "beers");

            migrationBuilder.DropIndex(
                name: "IX_tanks_Name",
                table: "tanks");

            migrationBuilder.CreateIndex(
                name: "IX_beers_Name",
                table: "beers",
                column: "Name",
                unique: true,
                filter: "\"IsActive\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_tanks_Name",
                table: "tanks",
                column: "Name",
                unique: true,
                filter: "\"IsActive\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_beers_Name",
                table: "beers");

            migrationBuilder.DropIndex(
                name: "IX_tanks_Name",
                table: "tanks");

            migrationBuilder.CreateIndex(
                name: "IX_beers_Name",
                table: "beers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tanks_Name",
                table: "tanks",
                column: "Name",
                unique: true);
        }
    }
}
