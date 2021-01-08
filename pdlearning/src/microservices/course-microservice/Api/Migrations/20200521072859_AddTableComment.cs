using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddTableComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseApprovalComment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseApprovalCommentBy",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseContentApprovalComment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseContentApprovalCommentBy",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApprovalComment",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ApprovalCommentBy",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ContentApprovalComment",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ContentApprovalCommentBy",
                table: "ClassRun");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Action",
                table: "Comment",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ObjectId",
                table: "Comment",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "CourseApprovalComment",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseApprovalCommentBy",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseContentApprovalComment",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseContentApprovalCommentBy",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalComment",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalCommentBy",
                table: "ClassRun",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentApprovalComment",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContentApprovalCommentBy",
                table: "ClassRun",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
