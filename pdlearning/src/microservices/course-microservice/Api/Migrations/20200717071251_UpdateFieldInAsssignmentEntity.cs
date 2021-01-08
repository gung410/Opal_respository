using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFieldInAsssignmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted_Order",
                table: "QuizAssignmentFormQuestion");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "QuizAssignmentFormQuestion");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Assignment");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "QuizAssignmentFormQuestion",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RandomizedOptions",
                table: "QuizAssignmentFormQuestion",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RandomizedQuestions",
                table: "Assignment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted_Priority",
                table: "QuizAssignmentFormQuestion",
                columns: new[] { "IsDeleted", "Priority" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted_Priority",
                table: "QuizAssignmentFormQuestion");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "QuizAssignmentFormQuestion");

            migrationBuilder.DropColumn(
                name: "RandomizedOptions",
                table: "QuizAssignmentFormQuestion");

            migrationBuilder.DropColumn(
                name: "RandomizedQuestions",
                table: "Assignment");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QuizAssignmentFormQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Assignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted_Order",
                table: "QuizAssignmentFormQuestion",
                columns: new[] { "IsDeleted", "Order" });
        }
    }
}
