using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class SubmittedContentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lecture");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedContentDate",
                table: "Course",
                nullable: true);

            migrationBuilder.Sql("UPDATE dbo.Course SET SubmittedContentDate = CreatedDate WHERE ContentStatus != 'Draft' AND ContentStatus is not NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedContentDate",
                table: "Course");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Lecture",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
