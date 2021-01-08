using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddFeedbackColumnsToFormQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackCorrectAnswer",
                table: "SharedQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackWrongAnswer",
                table: "SharedQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackCorrectAnswer",
                table: "FormQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackWrongAnswer",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question_FeedbackCorrectAnswer",
                table: "SharedQuestions");

            migrationBuilder.DropColumn(
                name: "Question_FeedbackWrongAnswer",
                table: "SharedQuestions");

            migrationBuilder.DropColumn(
                name: "Question_FeedbackCorrectAnswer",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "Question_FeedbackWrongAnswer",
                table: "FormQuestions");
        }
    }
}
