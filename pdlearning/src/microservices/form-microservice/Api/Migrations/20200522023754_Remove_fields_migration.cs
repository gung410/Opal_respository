using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class Remove_fields_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "OptId",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "ScaleRatings",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "ScaleRatingAnswerValue",
                table: "FormQuestionAnswers");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "FormQuestions");

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "FormQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OptId",
                table: "FormQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScaleRatings",
                table: "FormQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScaleRatingAnswerValue",
                table: "FormQuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
