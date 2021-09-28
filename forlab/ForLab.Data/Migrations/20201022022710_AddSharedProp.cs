using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLab.Data.Migrations
{
    public partial class AddSharedProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Testing",
                table: "Tests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Product",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Product",
                table: "Instruments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Lookup",
                table: "Laboratories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Testing",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Product",
                table: "Instruments");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Lookup",
                table: "Laboratories");
        }
    }
}
