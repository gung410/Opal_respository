using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddColumnsAndTableSupportLearningMultipleTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "MyCourses",
                type: "varchar(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunId",
                table: "MyCourses",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasContentChanged",
                table: "MyCourses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationId",
                table: "MyCourses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "LecturesInMyCourse",
                type: "varchar(5)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    CourseName = table.Column<string>(maxLength: 2000, nullable: true),
                    LearningMode = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    CourseCode = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MOEOfficerId = table.Column<Guid>(nullable: true),
                    PDActivityType = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    SubmittedDate = table.Column<DateTime>(nullable: true),
                    CourseType = table.Column<string>(unicode: false, maxLength: 15, nullable: false, defaultValue: "New"),
                    MaxReLearningTimes = table.Column<int>(nullable: false),
                    FirstAdministratorId = table.Column<Guid>(nullable: true),
                    SecondAdministratorId = table.Column<Guid>(nullable: true),
                    PrimaryApprovingOfficerId = table.Column<Guid>(nullable: true),
                    AlternativeApprovingOfficerId = table.Column<Guid>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 30, nullable: false, defaultValue: "Draft"),
                    ContentStatus = table.Column<string>(unicode: false, maxLength: 30, nullable: false, defaultValue: "Draft"),
                    PublishedContentDate = table.Column<DateTime>(nullable: true),
                    SubmittedContentDate = table.Column<DateTime>(nullable: true),
                    Version = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    ApprovalDate = table.Column<DateTime>(nullable: true),
                    ApprovalContentDate = table.Column<DateTime>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: true),
                    ArchiveDate = table.Column<DateTime>(nullable: true),
                    Source = table.Column<string>(maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    ExpiredDate = table.Column<DateTime>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CourseId",
                table: "Course",
                column: "CourseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropColumn(
                name: "ClassRunId",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "HasContentChanged",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "LecturesInMyCourse");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "MyCourses",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldNullable: true);
        }
    }
}
