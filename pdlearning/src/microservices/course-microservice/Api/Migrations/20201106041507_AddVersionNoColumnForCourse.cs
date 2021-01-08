using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddVersionNoColumnForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WillArchiveCommunity",
                table: "Course",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "VersionNo",
                table: "Course",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
            Update Course
            set VersionNo = 1
            where CourseCode is not null");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VersionNo",
                table: "Course");

            migrationBuilder.AlterColumn<bool>(
                name: "WillArchiveCommunity",
                table: "Course",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
