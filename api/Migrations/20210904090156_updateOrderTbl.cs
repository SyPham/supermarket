using Microsoft.EntityFrameworkCore.Migrations;

namespace Supermarket.Migrations
{
    public partial class updateOrderTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TeamId",
                table: "Orders",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Teams_TeamId",
                table: "Orders",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Teams_TeamId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TeamId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Orders");
        }
    }
}
