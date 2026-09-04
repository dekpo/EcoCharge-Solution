using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoCharge.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargeSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChargingStationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VehicleIdentifier = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StoppedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentPowerKw = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    TotalEnergyKwh = table.Column<decimal>(type: "decimal(12,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargingStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    MaxPowerKw = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargingStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsumptionReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChargeSessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChargingStationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PowerKw = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    TotalEnergyKwh = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumptionReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeSessions_ChargingStationId",
                table: "ChargeSessions",
                column: "ChargingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionReadings_RecordedAtUtc",
                table: "ConsumptionReadings",
                column: "RecordedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeSessions");

            migrationBuilder.DropTable(
                name: "ChargingStations");

            migrationBuilder.DropTable(
                name: "ConsumptionReadings");
        }
    }
}
