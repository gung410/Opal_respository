using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCourseAutomateActivatedFieldForClassrunEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CourseAutomateActivated",
                table: "ClassRun",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CourseAutomateActivated_IsDeleted_CreatedDate",
                table: "ClassRun",
                columns: new[] { "CourseAutomateActivated", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CourseAutomateActivated_IsDeleted_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "CourseAutomateActivated",
                table: "ClassRun");
        }
    }
}
