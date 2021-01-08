using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCourseMetadataKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetadataKeys",
                table: "Course",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Course
                SET MetadataKeys = '[]'
                WHERE MetadataKeys IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetadataKeys",
                table: "Course");
        }
    }
}
