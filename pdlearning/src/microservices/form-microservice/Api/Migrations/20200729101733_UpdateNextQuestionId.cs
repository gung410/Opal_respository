using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class UpdateNextQuestionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextQuestionId",
                table: "FormQuestions");

            migrationBuilder.AddColumn<Guid>(
                name: "Question_NextQuestionId",
                table: "SharedQuestions",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SqRatingType",
                table: "Forms",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Question_NextQuestionId",
                table: "FormQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question_NextQuestionId",
                table: "SharedQuestions");

            migrationBuilder.DropColumn(
                name: "Question_NextQuestionId",
                table: "FormQuestions");

            migrationBuilder.AlterColumn<string>(
                name: "SqRatingType",
                table: "Forms",
                type: "varchar(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NextQuestionId",
                table: "FormQuestions",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
