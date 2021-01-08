using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewFieldsForEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Session",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseCoFacilitatorIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalCode",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunVenueId",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "CourseCoFacilitatorIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ExternalCode",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ClassRunVenueId",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "ClassRun");
        }
    }
}
