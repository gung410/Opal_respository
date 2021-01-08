using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddFieldForRescheduleSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RescheduleEndDateTime",
                table: "Session",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RescheduleStartDateTime",
                table: "Session",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RescheduleEndDateTime",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "RescheduleStartDateTime",
                table: "Session");
        }
    }
}
