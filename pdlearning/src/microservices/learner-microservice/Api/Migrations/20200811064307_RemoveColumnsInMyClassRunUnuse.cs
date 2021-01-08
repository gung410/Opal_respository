using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RemoveColumnsInMyClassRunUnuse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratorComment",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "AdministratorCommentBy",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "CommentBy",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "ReasonBy",
                table: "MyClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorComment",
                table: "MyClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorCommentBy",
                table: "MyClassRun",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MyClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentBy",
                table: "MyClassRun",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MyClassRun",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReasonBy",
                table: "MyClassRun",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
