using Microsoft.EntityFrameworkCore.Migrations;

namespace Supermarket.Migrations
{
    public partial class UpdateAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateIndex(
            //    name: "IX_Accounts_Group_ID",
            //    table: "Accounts",
            //    column: "Group_ID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Accounts_Team_ID",
            //    table: "Accounts",
            //    column: "Team_ID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Accounts_Groups_Group_ID",
            //    table: "Accounts",
            //    column: "Group_ID",
            //    principalTable: "Groups",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Accounts_Teams_Team_ID",
            //    table: "Accounts",
            //    column: "Team_ID",
            //    principalTable: "Teams",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Groups_Group_ID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Teams_Team_ID",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Group_ID",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Team_ID",
                table: "Accounts");
        }
    }
}
