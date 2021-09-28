using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForkEat.Web.Migrations
{
    public partial class stock2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Units_UnitId",
                table: "Stocks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Stocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "Stocks",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<DateTime>(
                name: "BestBeforeDate",
                table: "Stocks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Stocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Stocks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId",
                table: "Stocks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Units_UnitId",
                table: "Stocks",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Units_UnitId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ProductId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "BestBeforeDate",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Stocks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Stocks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<float>(
                name: "Quantity",
                table: "Stocks",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Units_UnitId",
                table: "Stocks",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
