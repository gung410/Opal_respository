using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class Add_CourseCriteriaViolated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Users",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "Internal");

            migrationBuilder.AddColumn<bool>(
                name: "CourseCriteriaOverrided",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CourseCriteriaViolated",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseCriteriaViolation",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CourseCriteriaActivated",
                table: "ClassRun",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CourseCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    AccountType = table.Column<string>(unicode: false, maxLength: 15, nullable: false),
                    Tracks = table.Column<string>(nullable: true),
                    DevRoles = table.Column<string>(nullable: true),
                    TeachingLevels = table.Column<string>(nullable: true),
                    TeachingCourseOfStudy = table.Column<string>(nullable: true),
                    JobFamily = table.Column<string>(nullable: true),
                    CoCurricularActivity = table.Column<string>(nullable: true),
                    SubGradeBanding = table.Column<string>(nullable: true),
                    CourseCriteriaServiceSchemes = table.Column<string>(nullable: true),
                    PlaceOfWork = table.Column<string>(nullable: true),
                    PreRequisiteCourses = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUnitTypeDepartment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    DepartmentUnitTypeId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUnitTypeDepartment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registration_CourseCriteriaViolated_IsDeleted_CreatedDate",
                table: "Registration",
                columns: new[] { "CourseCriteriaViolated", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CourseCriteriaActivated_IsDeleted_CreatedDate",
                table: "ClassRun",
                columns: new[] { "CourseCriteriaActivated", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCriteria_IsDeleted",
                table: "CourseCriteria",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCriteria_AccountType_IsDeleted",
                table: "CourseCriteria",
                columns: new[] { "AccountType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUnitTypeDepartment_DepartmentId",
                table: "DepartmentUnitTypeDepartment",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUnitTypeDepartment_DepartmentUnitTypeId_IsDeleted_CreatedDate",
                table: "DepartmentUnitTypeDepartment",
                columns: new[] { "DepartmentUnitTypeId", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCriteria");

            migrationBuilder.DropTable(
                name: "DepartmentUnitTypeDepartment");

            migrationBuilder.DropIndex(
                name: "IX_Registration_CourseCriteriaViolated_IsDeleted_CreatedDate",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CourseCriteriaActivated_IsDeleted_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CourseCriteriaOverrided",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CourseCriteriaViolated",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CourseCriteriaViolation",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CourseCriteriaActivated",
                table: "ClassRun");
        }
    }
}
