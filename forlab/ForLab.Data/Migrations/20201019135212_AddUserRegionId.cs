using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLab.Data.Migrations
{
    public partial class AddUserRegionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "Security",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "Security",
                table: "Users");
        }
    }
}
