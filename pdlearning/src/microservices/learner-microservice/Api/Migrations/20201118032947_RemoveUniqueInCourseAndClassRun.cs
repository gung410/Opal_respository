using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RemoveUniqueInCourseAndClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_CourseId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ClassRunId",
                table: "ClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Course_CourseId",
                table: "Course",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ClassRunId",
                table: "ClassRun",
                column: "ClassRunId",
                unique: true);
        }
    }
}
