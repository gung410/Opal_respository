using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCommentColumnsForCourseAndRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorComment",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorCommentBy",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentBy",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReasonBy",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseApprovalComment",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseApprovalCommentBy",
                table: "Course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratorComment",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "AdministratorCommentBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CommentBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "ReasonBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CourseApprovalComment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseApprovalCommentBy",
                table: "Course");
        }
    }
}
