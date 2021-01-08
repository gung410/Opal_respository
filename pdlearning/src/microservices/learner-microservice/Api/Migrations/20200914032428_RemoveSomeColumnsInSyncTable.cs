using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RemoveSomeColumnsInSyncTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassRunId",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunId",
                table: "MyCourses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
