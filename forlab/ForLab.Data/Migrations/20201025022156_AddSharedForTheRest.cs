using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLab.Data.Migrations
{
    public partial class AddSharedForTheRest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Lookup",
                table: "TestingAreas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Lookup",
                table: "PatientGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Lookup",
                table: "LaboratoryLevels",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                schema: "Lookup",
                table: "LaboratoryCategories",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Lookup",
                table: "TestingAreas");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Lookup",
                table: "PatientGroups");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Lookup",
                table: "LaboratoryLevels");

            migrationBuilder.DropColumn(
                name: "Shared",
                schema: "Lookup",
                table: "LaboratoryCategories");
        }
    }
}
