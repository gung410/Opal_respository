using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddColumnIsUneditable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Question_IsUneditable",
                table: "SharedQuestions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Question_IsUneditable",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question_IsUneditable",
                table: "SharedQuestions");

            migrationBuilder.DropColumn(
                name: "Question_IsUneditable",
                table: "FormQuestions");
        }
    }
}
