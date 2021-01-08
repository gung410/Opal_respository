using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddingPassingMarksForForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "PassingMarkPercentage",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassingMarkScore",
                table: "Forms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassingMarkPercentage",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "PassingMarkScore",
                table: "Forms");
        }
    }
}
