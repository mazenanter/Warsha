using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedWorkshop_Clients_ClientId",
                table: "SavedWorkshop");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedWorkshop_Workshops_WorkshopId",
                table: "SavedWorkshop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedWorkshop",
                table: "SavedWorkshop");

            migrationBuilder.RenameTable(
                name: "SavedWorkshop",
                newName: "SavedWorkshops");

            migrationBuilder.RenameIndex(
                name: "IX_SavedWorkshop_WorkshopId",
                table: "SavedWorkshops",
                newName: "IX_SavedWorkshops_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedWorkshop_ClientId_WorkshopId",
                table: "SavedWorkshops",
                newName: "IX_SavedWorkshops_ClientId_WorkshopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedWorkshops",
                table: "SavedWorkshops",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedWorkshops_Clients_ClientId",
                table: "SavedWorkshops",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedWorkshops_Workshops_WorkshopId",
                table: "SavedWorkshops",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedWorkshops_Clients_ClientId",
                table: "SavedWorkshops");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedWorkshops_Workshops_WorkshopId",
                table: "SavedWorkshops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedWorkshops",
                table: "SavedWorkshops");

            migrationBuilder.RenameTable(
                name: "SavedWorkshops",
                newName: "SavedWorkshop");

            migrationBuilder.RenameIndex(
                name: "IX_SavedWorkshops_WorkshopId",
                table: "SavedWorkshop",
                newName: "IX_SavedWorkshop_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_SavedWorkshops_ClientId_WorkshopId",
                table: "SavedWorkshop",
                newName: "IX_SavedWorkshop_ClientId_WorkshopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedWorkshop",
                table: "SavedWorkshop",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedWorkshop_Clients_ClientId",
                table: "SavedWorkshop",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedWorkshop_Workshops_WorkshopId",
                table: "SavedWorkshop",
                column: "WorkshopId",
                principalTable: "Workshops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
