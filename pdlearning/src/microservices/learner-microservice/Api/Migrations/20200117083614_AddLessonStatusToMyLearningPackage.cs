using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddLessonStatusToMyLearningPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LessonStatus",
                table: "MyLearningPackages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LessonStatus",
                table: "MyLearningPackages");
        }
    }
}
