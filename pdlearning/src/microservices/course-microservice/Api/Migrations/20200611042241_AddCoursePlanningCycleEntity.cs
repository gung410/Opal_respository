using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCoursePlanningCycleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoursePlanningCycleId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "Course",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoursePlanningCycle",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    YearCycle = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePlanningCycle", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_VerifiedDate",
                table: "Course",
                column: "VerifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlanningCycle_EndDate",
                table: "CoursePlanningCycle",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlanningCycle_IsDeleted",
                table: "CoursePlanningCycle",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlanningCycle_StartDate",
                table: "CoursePlanningCycle",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlanningCycle_YearCycle",
                table: "CoursePlanningCycle",
                column: "YearCycle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePlanningCycle");

            migrationBuilder.DropIndex(
                name: "IX_Course_VerifiedDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CoursePlanningCycleId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "Course");
        }
    }
}
