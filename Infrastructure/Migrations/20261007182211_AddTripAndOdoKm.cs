using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripAndOdoKm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentKm",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "ActualOdometerKm",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "GpsAccumulatedKm",
                table: "Cars",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "GpsMileageLastUpdatedAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OdometerCorrections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    PreviousKm = table.Column<int>(type: "int", nullable: false),
                    NewKm = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CorrectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdometerCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdometerCorrections_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartLatitude = table.Column<double>(type: "float", nullable: false),
                    StartLongitude = table.Column<double>(type: "float", nullable: false),
                    EndLatitude = table.Column<double>(type: "float", nullable: true),
                    EndLongitude = table.Column<double>(type: "float", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DistanceKm = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    OBDDeviceId = table.Column<int>(type: "int", nullable: true),
                    FuelConsumedL = table.Column<float>(type: "real", nullable: true),
                    AvgSpeedKmh = table.Column<float>(type: "real", nullable: true),
                    DrivingScore = table.Column<float>(type: "real", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OdometerCorrections_CarId",
                table: "OdometerCorrections",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CarId",
                table: "Trips",
                column: "CarId",
                unique: true,
                filter: "[Status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CarId_Status",
                table: "Trips",
                columns: new[] { "CarId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OdometerCorrections");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropColumn(
                name: "ActualOdometerKm",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "GpsAccumulatedKm",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "GpsMileageLastUpdatedAt",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "CurrentKm",
                table: "Cars",
                type: "int",
                nullable: true);
        }
    }
}
