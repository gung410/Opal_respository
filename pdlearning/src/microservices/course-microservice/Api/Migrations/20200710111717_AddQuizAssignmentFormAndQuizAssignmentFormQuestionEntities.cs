using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddQuizAssignmentFormAndQuizAssignmentFormQuestionEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizAssignmentForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAssignmentForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizAssignmentFormQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    QuizAssignmentFormId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    MaxScore = table.Column<float>(nullable: false),
                    Question_Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    Question_Title = table.Column<string>(type: "NTEXT", maxLength: 20000, nullable: true),
                    Question_CorrectAnswer = table.Column<string>(nullable: true),
                    Question_Options = table.Column<string>(nullable: true),
                    Question_Hint = table.Column<string>(nullable: true),
                    Question_AnswerExplanatoryNote = table.Column<string>(nullable: true),
                    Question_FeedbackCorrectAnswer = table.Column<string>(nullable: true),
                    Question_FeedbackWrongAnswer = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAssignmentFormQuestion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentForm_IsDeleted",
                table: "QuizAssignmentForm",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted",
                table: "QuizAssignmentFormQuestion",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_QuizAssignmentFormId",
                table: "QuizAssignmentFormQuestion",
                column: "QuizAssignmentFormId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_IsDeleted_Order",
                table: "QuizAssignmentFormQuestion",
                columns: new[] { "IsDeleted", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAssignmentFormQuestion_Question_Type_IsDeleted",
                table: "QuizAssignmentFormQuestion",
                columns: new[] { "Question_Type", "IsDeleted" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizAssignmentForm");

            migrationBuilder.DropTable(
                name: "QuizAssignmentFormQuestion");
        }
    }
}
