using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddLectureQuizConfigEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QuizConfig_ByPassPassingRate",
                table: "LectureContent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizConfig_ByPassPassingRate",
                table: "LectureContent");
        }
    }
}
