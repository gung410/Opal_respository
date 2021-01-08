using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateLazyloadingForPrerequisiteCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedDate_IsDeleted_Status",
                table: "Course",
                columns: new[] { "CreatedDate", "IsDeleted", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedDate_IsDeleted_Status",
                table: "Course");
        }
    }
}
