using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RemoveContentApprovalCommentAndContentApprovalCommentByInClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentApprovalComment",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "ContentApprovalCommentBy",
                table: "ClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
