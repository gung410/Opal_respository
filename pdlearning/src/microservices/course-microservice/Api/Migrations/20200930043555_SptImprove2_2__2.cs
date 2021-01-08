using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class SptImprove2_2__2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_CourseId_Value_Type",
                table: "CourseInternalValue",
                columns: new[] { "CourseId", "Value", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_CourseId_Value_Type",
                table: "CourseInternalValue");
        }
    }
}
