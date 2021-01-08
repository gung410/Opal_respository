using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddIsCodeScannedFieldForAttendanceTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeScannedDate",
                table: "AttendanceTracking",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCodeScanned",
                table: "AttendanceTracking",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeScannedDate",
                table: "AttendanceTracking");

            migrationBuilder.DropColumn(
                name: "IsCodeScanned",
                table: "AttendanceTracking");
        }
    }
}
