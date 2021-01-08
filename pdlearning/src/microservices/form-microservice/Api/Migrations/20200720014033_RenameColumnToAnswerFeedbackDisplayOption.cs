using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class RenameColumnToAnswerFeedbackDisplayOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackAnswerDisplayOption",
                table: "Forms");

            migrationBuilder.AddColumn<string>(
                name: "AnswerFeedbackDisplayOption",
                table: "Forms",
                type: "varchar(30)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerFeedbackDisplayOption",
                table: "Forms");

            migrationBuilder.AddColumn<string>(
                name: "FeedbackAnswerDisplayOption",
                table: "Forms",
                type: "varchar(30)",
                nullable: true);
        }
    }
}
