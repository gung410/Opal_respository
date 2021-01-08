using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddColumnsForChangeClassRunInMyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunChangeId",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassRunChangeRequestedDate",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassRunChangeStatus",
                table: "MyClassRun",
                type: "varchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassRunChangeId",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "ClassRunChangeRequestedDate",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "ClassRunChangeStatus",
                table: "MyClassRun");
        }
    }
}
