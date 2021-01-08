using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class MoveRandomizedQuestionsToQuizAssignmentFormEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomizedQuestions",
                table: "Assignment");

            migrationBuilder.AddColumn<bool>(
                name: "RandomizedQuestions",
                table: "QuizAssignmentForm",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomizedQuestions",
                table: "QuizAssignmentForm");

            migrationBuilder.AddColumn<bool>(
                name: "RandomizedQuestions",
                table: "Assignment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
