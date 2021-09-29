using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class stepsAndIngredientsAsDbSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Products_ProductId",
                table: "IngredientEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                table: "IngredientEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StepEntity_Recipes_RecipeEntityId",
                table: "StepEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StepEntity",
                table: "StepEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientEntity",
                table: "IngredientEntity");

            migrationBuilder.RenameTable(
                name: "StepEntity",
                newName: "Steps");

            migrationBuilder.RenameTable(
                name: "IngredientEntity",
                newName: "Ingredients");

            migrationBuilder.RenameIndex(
                name: "IX_StepEntity_RecipeEntityId",
                table: "Steps",
                newName: "IX_Steps_RecipeEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientEntity_RecipeEntityId",
                table: "Ingredients",
                newName: "IX_Ingredients_RecipeEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientEntity_ProductId",
                table: "Ingredients",
                newName: "IX_Ingredients_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Steps",
                table: "Steps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Products_ProductId",
                table: "Ingredients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_RecipeEntityId",
                table: "Ingredients",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Recipes_RecipeEntityId",
                table: "Steps",
                column: "RecipeEntityId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Products_ProductId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_RecipeEntityId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Recipes_RecipeEntityId",
                table: "Steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Steps",
                table: "Steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients");

            migrationBuilder.RenameTable(
                name: "Steps",
                newName: "StepEntity");

            migrationBuilder.RenameTable(
                name: "Ingredients",
                newName: "IngredientEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Steps_RecipeEntityId",
                table: "StepEntity",
                newName: "IX_StepEntity_RecipeEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_RecipeEntityId",
                table: "IngredientEntity",
                newName: "IX_IngredientEntity_RecipeEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_ProductId",
                table: "IngredientEntity",
                newName: "IX_IngredientEntity_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StepEntity",
                table: "StepEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientEntity",
                table: "IngredientEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntity_Products_ProductId",
                table: "IngredientEntity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
