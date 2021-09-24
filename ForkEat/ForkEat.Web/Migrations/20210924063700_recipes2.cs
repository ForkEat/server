using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class recipes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                table: "IngredientEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId1",
                table: "IngredientEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId",
                table: "StepEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId1",
                table: "StepEntity");

            migrationBuilder.DropIndex(
                name: "IX_StepEntity_RecipeEntityId1",
                table: "StepEntity");

            migrationBuilder.DropIndex(
                name: "IX_IngredientEntity_RecipeEntityId1",
                table: "IngredientEntity");

            migrationBuilder.DropColumn(
                name: "RecipeEntityId1",
                table: "StepEntity");

            migrationBuilder.DropColumn(
                name: "RecipeEntityId1",
                table: "IngredientEntity");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                table: "IngredientEntity",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId",
                table: "StepEntity",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                table: "IngredientEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId",
                table: "StepEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeEntityId1",
                table: "StepEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeEntityId1",
                table: "IngredientEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepEntity_RecipeEntityId1",
                table: "StepEntity",
                column: "RecipeEntityId1");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_RecipeEntityId1",
                table: "IngredientEntity",
                column: "RecipeEntityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                table: "IngredientEntity",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId1",
                table: "IngredientEntity",
                column: "RecipeEntityId1",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId",
                table: "StepEntity",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId1",
                table: "StepEntity",
                column: "RecipeEntityId1",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
