using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewFieldsForResgistrationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LearningContentProgress",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningStatus",
                table: "Registration",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.AddColumn<bool>(
                name: "PostCourseEvaluationFormCompleted",
                table: "Registration",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningContentProgress",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "LearningStatus",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "PostCourseEvaluationFormCompleted",
                table: "Registration");
        }
    }
}
