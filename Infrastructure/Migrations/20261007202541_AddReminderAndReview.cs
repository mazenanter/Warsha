using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderAndReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Clients_ClientId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Workshops_WorkshopId",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_Review_WorkshopId",
                table: "Reviews",
                newName: "IX_Reviews_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ClientId",
                table: "Reviews",
                newName: "IX_Reviews_ClientId");

            migrationBuilder.AddColumn<int>(
                name: "CarBrandId",
                table: "Specializations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarModelId",
                table: "Specializations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reminders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SavedWorkshop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    WorkshopId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedWorkshop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedWorkshop_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedWorkshop_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_CarBrandId",
                table: "Specializations",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_CarModelId",
                table: "Specializations",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_CarId",
                table: "Reminders",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_ClientId",
                table: "Reminders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedWorkshop_ClientId_WorkshopId",
                table: "SavedWorkshop",
                columns: new[] { "ClientId", "WorkshopId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedWorkshop_WorkshopId",
                table: "SavedWorkshop",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Clients_ClientId",
                table: "Reviews",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Workshops_WorkshopId",
                table: "Reviews",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Specializations_CarBrands_CarBrandId",
                table: "Specializations",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Specializations_CarModels_CarModelId",
                table: "Specializations",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Clients_ClientId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Workshops_WorkshopId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Specializations_CarBrands_CarBrandId",
                table: "Specializations");

            migrationBuilder.DropForeignKey(
                name: "FK_Specializations_CarModels_CarModelId",
                table: "Specializations");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "SavedWorkshop");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_CarBrandId",
                table: "Specializations");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_CarModelId",
                table: "Specializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CarBrandId",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "CarModelId",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_WorkshopId",
                table: "Review",
                newName: "IX_Review_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ClientId",
                table: "Review",
                newName: "IX_Review_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Clients_ClientId",
                table: "Review",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Workshops_WorkshopId",
                table: "Review",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
