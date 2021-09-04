using Microsoft.EntityFrameworkCore.Migrations;

namespace Supermarket.Migrations
{
    public partial class updateOrderDetailTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "OrderDetailHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TeamId",
                table: "OrderDetails",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailHistories_TeamId",
                table: "OrderDetailHistories",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailHistories_Teams_TeamId",
                table: "OrderDetailHistories",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Teams_TeamId",
                table: "OrderDetails",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailHistories_Teams_TeamId",
                table: "OrderDetailHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Teams_TeamId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_TeamId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetailHistories_TeamId",
                table: "OrderDetailHistories");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "OrderDetailHistories");

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
    }
}
