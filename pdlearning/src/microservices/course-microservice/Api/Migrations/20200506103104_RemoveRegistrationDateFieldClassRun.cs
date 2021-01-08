using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveRegistrationDateFieldClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationEndDate",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "RegistrationStartDate",
                table: "ClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationEndDate",
                table: "ClassRun",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationStartDate",
                table: "ClassRun",
                type: "datetime2",
                nullable: true);
        }
    }
}
