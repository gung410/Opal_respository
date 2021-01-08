using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class Adding_Header_OptId_FormQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "FormQuestions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OptId",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "OptId",
                table: "FormQuestions");
        }
    }
}
