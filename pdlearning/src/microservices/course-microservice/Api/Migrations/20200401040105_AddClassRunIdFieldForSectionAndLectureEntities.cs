using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddClassRunIdFieldForSectionAndLectureEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunId",
                table: "Section",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunId",
                table: "Lecture",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassRunId",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "ClassRunId",
                table: "Lecture");
        }
    }
}
