using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCourseAndClassRunContentStatusField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentStatus",
                table: "Course",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.AddColumn<string>(
                name: "ContentStatus",
                table: "ClassRun",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Draft");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentStatus",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ContentStatus",
                table: "ClassRun");
        }
    }
}
