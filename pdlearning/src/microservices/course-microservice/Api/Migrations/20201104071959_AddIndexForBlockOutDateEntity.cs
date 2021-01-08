using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddIndexForBlockOutDateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_PlanningCycleId",
                table: "BlockoutDate");

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_PlanningCycleId_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "PlanningCycleId", "IsDeleted", "FullTextSearchKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_PlanningCycleId_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_PlanningCycleId",
                table: "BlockoutDate",
                column: "PlanningCycleId");
        }
    }
}
