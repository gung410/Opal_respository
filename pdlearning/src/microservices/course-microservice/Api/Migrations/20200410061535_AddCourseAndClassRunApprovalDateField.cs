using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCourseAndClassRunApprovalDateField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalContentDate",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalContentDate",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalContentDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApprovalContentDate",
                table: "ClassRun");
        }
    }
}
