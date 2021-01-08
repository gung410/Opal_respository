using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddCourseTypeAndMyRegistrationStatusToMyCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseType",
                table: "MyCourses",
                type: "varchar(30)",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "MyRegistrationStatus",
                table: "MyCourses",
                type: "varchar(30)",
                nullable: false,
                defaultValue: string.Empty);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "MyRegistrationStatus",
                table: "MyCourses");
        }
    }
}
