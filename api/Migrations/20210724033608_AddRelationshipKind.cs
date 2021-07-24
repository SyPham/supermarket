using Microsoft.EntityFrameworkCore.Migrations;

namespace Supermarket.Migrations
{
    public partial class AddRelationshipKind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Kinds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kinds_StoreId",
                table: "Kinds",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kinds_Stores_StoreId",
                table: "Kinds",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kinds_Stores_StoreId",
                table: "Kinds");

            migrationBuilder.DropIndex(
                name: "IX_Kinds_StoreId",
                table: "Kinds");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Kinds");
        }
    }
}
