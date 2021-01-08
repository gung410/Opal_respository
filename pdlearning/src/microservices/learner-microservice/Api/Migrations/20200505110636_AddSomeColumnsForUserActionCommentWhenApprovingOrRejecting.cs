using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddSomeColumnsForUserActionCommentWhenApprovingOrRejecting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorComment",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorCommentBy",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentBy",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReasonBy",
                table: "MyClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratorComment",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "AdministratorCommentBy",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "CommentBy",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "ReasonBy",
                table: "MyClassRun");
        }
    }
}
