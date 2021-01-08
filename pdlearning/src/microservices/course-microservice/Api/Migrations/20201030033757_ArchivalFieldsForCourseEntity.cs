using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class ArchivalFieldsForCourseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArchivedBy",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WillArchiveCommunity",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_ArchiveDate",
                table: "Course",
                columns: new[] { "IsDeleted", "ArchiveDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_ArchiveDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "WillArchiveCommunity",
                table: "Course");
        }
    }
}
