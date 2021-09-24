using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class recipes3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Products_ProductId1",
                table: "IngredientEntity");

            migrationBuilder.DropIndex(
                name: "IX_IngredientEntity_ProductId1",
                table: "IngredientEntity");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "IngredientEntity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "IngredientEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_ProductId1",
                table: "IngredientEntity",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntity_Products_ProductId1",
                table: "IngredientEntity",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
