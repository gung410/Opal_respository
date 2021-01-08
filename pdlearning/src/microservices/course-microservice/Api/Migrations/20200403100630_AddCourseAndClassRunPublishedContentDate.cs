using System;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCourseAndClassRunPublishedContentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedContentDate",
                table: "Course",
                nullable: true);
            migrationBuilder.Sql($"UPDATE Course SET PublishedContentDate = ChangedDate WHERE ContentStatus = '{ContentStatus.Published}' AND PublishedContentDate is NULL AND Status = '{CourseStatus.Published}'");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedContentDate",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedContentDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PublishedContentDate",
                table: "ClassRun");
        }
    }
}
