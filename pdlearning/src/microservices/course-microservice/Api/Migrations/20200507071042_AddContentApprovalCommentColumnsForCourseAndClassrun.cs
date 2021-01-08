using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddContentApprovalCommentColumnsForCourseAndClassrun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseContentApprovalComment",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseContentApprovalCommentBy",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentApprovalComment",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContentApprovalCommentBy",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseContentApprovalComment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseContentApprovalCommentBy",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ContentApprovalComment",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ContentApprovalCommentBy",
                table: "ClassRun");
        }
    }
}
