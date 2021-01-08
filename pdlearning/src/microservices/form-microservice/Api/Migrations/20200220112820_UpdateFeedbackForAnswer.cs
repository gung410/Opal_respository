using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class UpdateFeedbackForAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Question_AnswerExplanatoryNote", "FormQuestions", "Question_FeedbackOnCorrectAnswers");

            migrationBuilder.RenameColumn("Question_AnswerExplanatoryNote", "SharedQuestions", "Question_FeedbackOnCorrectAnswers");

            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackOnWrongAnswers",
                table: "SharedQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question_FeedbackOnWrongAnswers",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question_FeedbackOnWrongAnswers",
                table: "SharedQuestions");

            migrationBuilder.DropColumn(
                name: "Question_FeedbackOnWrongAnswers",
                table: "FormQuestions");

            migrationBuilder.RenameColumn("Question_FeedbackOnCorrectAnswers", "FormQuestions", "Question_AnswerExplanatoryNote");

            migrationBuilder.RenameColumn("Question_FeedbackOnCorrectAnswers", "SharedQuestions", "Question_AnswerExplanatoryNote");
        }
    }
}
