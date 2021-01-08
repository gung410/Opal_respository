using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.LnaForm.Migrations
{
    public partial class RemoveSurveyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSurveyTemplate",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "SurveyType",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Forms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSurveyTemplate",
                table: "Forms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurveyType",
                table: "Forms",
                type: "varchar(30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Forms",
                type: "varchar(30)",
                nullable: false,
                defaultValue: string.Empty);
        }
    }
}
