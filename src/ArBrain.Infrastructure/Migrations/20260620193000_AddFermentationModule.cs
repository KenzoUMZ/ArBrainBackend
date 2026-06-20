using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArBrain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFermentationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_beers_Name",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "Abv",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "MinimumStock",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "beers");

            migrationBuilder.CreateIndex(
                name: "IX_beers_Name",
                table: "beers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateTable(
                name: "beer_fermentation_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BeerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinTemperature = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MaxTemperature = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MinPh = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    MaxPh = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    MinExtract = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    MaxExtract = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beer_fermentation_parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_beer_fermentation_parameters_beers_BeerId",
                        column: x => x.BeerId,
                        principalTable: "beers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CapacityLiters = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fermentation_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BeerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TankId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Temperature = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Ph = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    Extract = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    Observations = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ComplianceStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fermentation_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fermentation_records_beers_BeerId",
                        column: x => x.BeerId,
                        principalTable: "beers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fermentation_records_tanks_TankId",
                        column: x => x.TankId,
                        principalTable: "tanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beer_fermentation_parameters_BeerId",
                table: "beer_fermentation_parameters",
                column: "BeerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fermentation_records_BatchNumber",
                table: "fermentation_records",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_fermentation_records_BeerId",
                table: "fermentation_records",
                column: "BeerId");

            migrationBuilder.CreateIndex(
                name: "IX_fermentation_records_ComplianceStatus",
                table: "fermentation_records",
                column: "ComplianceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_fermentation_records_RegisteredAt",
                table: "fermentation_records",
                column: "RegisteredAt");

            migrationBuilder.CreateIndex(
                name: "IX_fermentation_records_TankId",
                table: "fermentation_records",
                column: "TankId");

            migrationBuilder.CreateIndex(
                name: "IX_tanks_IsActive",
                table: "tanks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_tanks_Name",
                table: "tanks",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "beer_fermentation_parameters");
            migrationBuilder.DropTable(name: "fermentation_records");
            migrationBuilder.DropTable(name: "tanks");

            migrationBuilder.DropIndex(name: "IX_beers_Name", table: "beers");

            migrationBuilder.AddColumn<decimal>(
                name: "Abv",
                table: "beers",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MinimumStock",
                table: "beers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "beers",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "beers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_beers_Name",
                table: "beers",
                column: "Name");
        }
    }
}
