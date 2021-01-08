using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class ImproveSpt22_UseFullTextSearchForMultiValuesColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CollaborativeContentCreatorIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseCoFacilitatorIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseFacilitatorIdsFullTextSearch",
                table: "Course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollaborativeContentCreatorIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseCoFacilitatorIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseFacilitatorIdsFullTextSearch",
                table: "Course");
        }
    }
}
