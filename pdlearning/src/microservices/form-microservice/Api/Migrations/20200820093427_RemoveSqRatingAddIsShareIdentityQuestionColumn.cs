using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class RemoveSqRatingAddIsShareIdentityQuestionColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Satisfaction",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "SqRating",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "Usefulness",
                table: "FormAnswers");

            migrationBuilder.AddColumn<bool>(
                name: "IsShareIdentityQuestion",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShareIdentityQuestion",
                table: "FormQuestions");

            migrationBuilder.AddColumn<double>(
                name: "Satisfaction",
                table: "FormAnswers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SqRating",
                table: "FormAnswers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Usefulness",
                table: "FormAnswers",
                type: "float",
                nullable: true);
        }
    }
}
