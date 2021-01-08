using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class Adding_ScaleRatings_Form : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScaleRatings",
                table: "FormQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScaleRatingAnswerValue",
                table: "FormQuestionAnswers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScaleRatings",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "ScaleRatingAnswerValue",
                table: "FormQuestionAnswers");
        }
    }
}
