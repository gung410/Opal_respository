using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddPlanningTimeToClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlanningEndTime",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanningStartTime",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanningEndTime",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "PlanningStartTime",
                table: "ClassRun");
        }
    }
}
