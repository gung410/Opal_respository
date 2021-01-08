using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class SptImprove2_2__1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_Value_Type_CourseId",
                table: "CourseInternalValue",
                columns: new[] { "Value", "Type", "CourseId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_Value_Type_CourseId",
                table: "CourseInternalValue");
        }
    }
}
