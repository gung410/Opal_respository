using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewInternalEntitiesForCourseAndClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassRunInternalValue",
                columns: table => new
                {
                    ClassRunId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValue: "FacilitatorIds"),
                    Value = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRunInternalValue", x => new { x.ClassRunId, x.Id });
                    table.ForeignKey(
                        name: "FK_ClassRunInternalValue_ClassRun_ClassRunId",
                        column: x => x.ClassRunId,
                        principalTable: "ClassRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseInternalValue",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValue: "CourseFacilitatorIds"),
                    Value = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInternalValue", x => new { x.CourseId, x.Id });
                    table.ForeignKey(
                        name: "FK_CourseInternalValue_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassRunInternalValue");

            migrationBuilder.DropTable(
                name: "CourseInternalValue");
        }
    }
}
