using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.LnaForm.Migrations
{
    public partial class RemoveRedundantFieldInForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllowedDisplayPollResult",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsShowFreeTextQuestionInPoll",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsStandalone",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ShowQuizSummary",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "StandaloneMode",
                table: "Forms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllowedDisplayPollResult",
                table: "Forms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowFreeTextQuestionInPoll",
                table: "Forms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStandalone",
                table: "Forms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowQuizSummary",
                table: "Forms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandaloneMode",
                table: "Forms",
                type: "varchar(30)",
                nullable: true);
        }
    }
}
