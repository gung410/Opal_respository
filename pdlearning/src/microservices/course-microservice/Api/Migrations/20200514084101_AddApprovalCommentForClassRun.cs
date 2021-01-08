using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddApprovalCommentForClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalComment",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalCommentBy",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalComment",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ApprovalCommentBy",
                table: "ClassRun");
        }
    }
}
