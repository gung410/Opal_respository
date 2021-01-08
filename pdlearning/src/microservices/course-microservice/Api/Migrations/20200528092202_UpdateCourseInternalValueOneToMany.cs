using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateCourseInternalValueOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseInternalValue",
                table: "CourseInternalValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseInternalValue",
                table: "CourseInternalValue",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseInternalValue",
                table: "CourseInternalValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseInternalValue",
                table: "CourseInternalValue",
                columns: new[] { "CourseId", "Id" });
        }
    }
}
