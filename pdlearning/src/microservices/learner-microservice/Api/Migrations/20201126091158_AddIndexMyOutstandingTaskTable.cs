using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddIndexMyOutstandingTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType",
                table: "MyOutstandingTasks",
                columns: new[] { "UserId", "ItemType" });

            migrationBuilder.CreateIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType_ItemId",
                table: "MyOutstandingTasks",
                columns: new[] { "UserId", "ItemType", "ItemId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType",
                table: "MyOutstandingTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType_ItemId",
                table: "MyOutstandingTasks");
        }
    }
}
