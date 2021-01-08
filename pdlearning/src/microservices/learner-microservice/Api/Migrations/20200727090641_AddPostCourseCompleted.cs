using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddPostCourseCompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PostCourseEvaluationFormCompleted",
                table: "MyCourses",
                nullable: true,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCourseEvaluationFormCompleted",
                table: "MyCourses");
        }
    }
}
