using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.LnaForm.Migrations
{
    public partial class RemoveRedundantStuffs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptToShowFeedback",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsSurveyTemplateQuestion",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "AnswerFeedback",
                table: "FormQuestionAnswers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "AttemptToShowFeedback",
                table: "Forms",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSurveyTemplateQuestion",
                table: "FormQuestions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerFeedback",
                table: "FormQuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
