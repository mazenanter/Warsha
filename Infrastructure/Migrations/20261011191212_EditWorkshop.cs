using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditWorkshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapsLink",
                table: "Workshops");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Workshops",
                newName: "AddressEn");

            migrationBuilder.AddColumn<string>(
                name: "AddressAr",
                table: "Workshops",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Workshops",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Workshops",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Workshops",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressAr",
                table: "Workshops");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Workshops");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Workshops");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Workshops");

            migrationBuilder.RenameColumn(
                name: "AddressEn",
                table: "Workshops",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsLink",
                table: "Workshops",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
