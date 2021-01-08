using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22_MyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MyClassRun_ClassRunId_IsDeleted",
                table: "MyClassRun",
                columns: new[] { "ClassRunId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_MyClassRun_ClassRunId_Status_IsExpired_IsDeleted",
                table: "MyClassRun",
                columns: new[] { "ClassRunId", "Status", "IsExpired", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_StartDateTime",
                table: "ClassRun",
                column: "StartDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyClassRun_ClassRunId_IsDeleted",
                table: "MyClassRun");

            migrationBuilder.DropIndex(
                name: "IX_MyClassRun_ClassRunId_Status_IsExpired_IsDeleted",
                table: "MyClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_StartDateTime",
                table: "ClassRun");
        }
    }
}
