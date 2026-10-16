using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopMonthlyActionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizationId",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDeadline",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServicesAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "WorkshopMonthlyActionLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkshopId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    ActionsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopMonthlyActionLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopMonthlyActionLimits_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopMonthlyActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkshopId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopMonthlyActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopMonthlyActions_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkshopMonthlyActions_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopMonthlyActionLimits_WorkshopId_Year_Month",
                table: "WorkshopMonthlyActionLimits",
                columns: new[] { "WorkshopId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopMonthlyActions_BookingId",
                table: "WorkshopMonthlyActions",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopMonthlyActions_WorkshopId_BookingId_ActionType",
                table: "WorkshopMonthlyActions",
                columns: new[] { "WorkshopId", "BookingId", "ActionType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopMonthlyActions_WorkshopId_Year_Month",
                table: "WorkshopMonthlyActions",
                columns: new[] { "WorkshopId", "Year", "Month" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkshopMonthlyActionLimits");

            migrationBuilder.DropTable(
                name: "WorkshopMonthlyActions");

            migrationBuilder.DropColumn(
                name: "AuthorizationId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ResponseDeadline",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServicesAmount",
                table: "Bookings");
        }
    }
}
