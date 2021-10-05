using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class stepsOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Order",
                table: "Steps",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Steps");
        }
    }
}
