using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateCourseAndClassRunInternalValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_CourseId",
                table: "CourseInternalValue",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_Type",
                table: "CourseInternalValue",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_Value",
                table: "CourseInternalValue",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRunInternalValue_ClassRunId",
                table: "ClassRunInternalValue",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRunInternalValue_Type",
                table: "ClassRunInternalValue",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRunInternalValue_Value",
                table: "ClassRunInternalValue",
                column: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_CourseId",
                table: "CourseInternalValue");

            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_Type",
                table: "CourseInternalValue");

            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_Value",
                table: "CourseInternalValue");

            migrationBuilder.DropIndex(
                name: "IX_ClassRunInternalValue_ClassRunId",
                table: "ClassRunInternalValue");

            migrationBuilder.DropIndex(
                name: "IX_ClassRunInternalValue_Type",
                table: "ClassRunInternalValue");

            migrationBuilder.DropIndex(
                name: "IX_ClassRunInternalValue_Value",
                table: "ClassRunInternalValue");
        }
    }
}
