using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddingFileds_FormEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignmentId",
                table: "FormAnswers",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassRunId",
                table: "FormAnswers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "ClassRunId",
                table: "FormAnswers");
        }
    }
}
