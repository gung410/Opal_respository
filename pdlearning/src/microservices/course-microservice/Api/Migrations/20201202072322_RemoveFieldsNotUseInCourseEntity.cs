using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveFieldsNotUseInCourseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseContent",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TermsOfUse",
                table: "Course");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseContent",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Course",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfUse",
                table: "Course",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
