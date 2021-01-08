using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddIndexesToMyCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MyCourses_UserId",
                table: "MyCourses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MyCourses_CourseId_UserId",
                table: "MyCourses",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_MyCourses_UserId_CourseId",
                table: "MyCourses",
                columns: new[] { "UserId", "CourseId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyCourses_UserId",
                table: "MyCourses");

            migrationBuilder.DropIndex(
                name: "IX_MyCourses_CourseId_UserId",
                table: "MyCourses");

            migrationBuilder.DropIndex(
                name: "IX_MyCourses_UserId_CourseId",
                table: "MyCourses");
        }
    }
}
