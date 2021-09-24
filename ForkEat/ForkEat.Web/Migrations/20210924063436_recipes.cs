using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class recipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Difficulty = table.Column<long>(type: "bigint", nullable: false),
                    TotalEstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    ProductId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipeEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipeEntityId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientEntity_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientEntity_Products_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientEntity_Recipes_RecipeEntityId",
                        column: x => x.RecipeEntityId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientEntity_Recipes_RecipeEntityId1",
                        column: x => x.RecipeEntityId1,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    EstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RecipeEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipeEntityId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepEntity_Recipes_RecipeEntityId",
                        column: x => x.RecipeEntityId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepEntity_Recipes_RecipeEntityId1",
                        column: x => x.RecipeEntityId1,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_ProductId",
                table: "IngredientEntity",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_ProductId1",
                table: "IngredientEntity",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_RecipeEntityId",
                table: "IngredientEntity",
                column: "RecipeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientEntity_RecipeEntityId1",
                table: "IngredientEntity",
                column: "RecipeEntityId1");

            migrationBuilder.CreateIndex(
                name: "IX_StepEntity_RecipeEntityId",
                table: "StepEntity",
                column: "RecipeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_StepEntity_RecipeEntityId1",
                table: "StepEntity",
                column: "RecipeEntityId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientEntity");

            migrationBuilder.DropTable(
                name: "StepEntity");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
