using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFieldsForLearningPathEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseLevelIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningAreaIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningDimensionIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningFrameworkIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningSubAreaIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PDAreaThemeIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSchemeIds",
                table: "LearningPath",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectAreaIds",
                table: "LearningPath",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseLevelIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "LearningAreaIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "LearningDimensionIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "LearningFrameworkIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "LearningSubAreaIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "PDAreaThemeIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "ServiceSchemeIds",
                table: "LearningPath");

            migrationBuilder.DropColumn(
                name: "SubjectAreaIds",
                table: "LearningPath");
        }
    }
}
