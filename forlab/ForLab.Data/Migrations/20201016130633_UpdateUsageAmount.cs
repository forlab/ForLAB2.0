using Microsoft.EntityFrameworkCore.Migrations;

namespace ForLab.Data.Migrations
{
    public partial class UpdateUsageAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "Product",
                table: "ProductUsage",
                type: "decimal(20,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)",
                oldDefaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "Product",
                table: "ProductUsage",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,8)",
                oldDefaultValue: 0m);
        }
    }
}
