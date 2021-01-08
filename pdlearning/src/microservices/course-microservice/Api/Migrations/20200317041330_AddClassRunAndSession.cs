using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddClassRunAndSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassRun",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    ClassTitle = table.Column<string>(maxLength: 2000, nullable: true),
                    ClassRunCode = table.Column<string>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: true),
                    EndDateTime = table.Column<DateTime>(nullable: true),
                    FacilitatorIds = table.Column<string>(nullable: true),
                    CoFacilitatorIds = table.Column<string>(nullable: true),
                    MinClassSize = table.Column<int>(nullable: false),
                    MaxClassSize = table.Column<int>(nullable: false),
                    ApplicationStartDate = table.Column<DateTime>(nullable: true),
                    ApplicationEndDate = table.Column<DateTime>(nullable: true),
                    RegistrationStartDate = table.Column<DateTime>(nullable: true),
                    RegistrationEndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 30, nullable: false, defaultValue: "Unpublished"),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRun", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    ClassRunId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassRun");

            migrationBuilder.DropTable(
                name: "Session");
        }
    }
}
