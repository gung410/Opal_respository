using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    SubmitDate = table.Column<DateTime>(nullable: true),
                    Score = table.Column<double>(nullable: true),
                    ScorePercentage = table.Column<double>(nullable: true),
                    Attempt = table.Column<short>(nullable: false),
                    FormMetaData = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    IsCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    FormAnswerId = table.Column<Guid>(nullable: false),
                    FormQuestionId = table.Column<Guid>(nullable: false),
                    AnswerValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxScore = table.Column<double>(nullable: true),
                    Score = table.Column<double>(nullable: true),
                    ScoredBy = table.Column<Guid>(nullable: true),
                    AnswerFeedback = table.Column<string>(nullable: true),
                    SubmittedDate = table.Column<DateTime>(nullable: true),
                    SpentTimeInSeconds = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestionAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    Question_Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    Question_Title = table.Column<string>(maxLength: 3000, nullable: true),
                    Question_CorrectAnswer = table.Column<string>(nullable: true),
                    Question_Options = table.Column<string>(nullable: true),
                    Question_Hint = table.Column<string>(nullable: true),
                    Question_AnswerExplanatoryNote = table.Column<string>(nullable: true),
                    Question_Level = table.Column<int>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(type: "nvarchar(3000)", nullable: true),
                    Priority = table.Column<int>(nullable: false, defaultValue: 0),
                    ShowFeedBackAfterAnswer = table.Column<bool>(nullable: true),
                    RandomizedOptions = table.Column<bool>(nullable: true),
                    Score = table.Column<double>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    LectureId = table.Column<Guid>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: true),
                    SectionId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DueDate = table.Column<DateTime>(nullable: true),
                    InSecondTimeLimit = table.Column<int>(nullable: true),
                    RandomizedQuestions = table.Column<bool>(nullable: false, defaultValue: false),
                    MaxAttempt = table.Column<short>(nullable: true),
                    ShowQuizSummary = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharedQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    Question_Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    Question_Title = table.Column<string>(maxLength: 3000, nullable: true),
                    Question_CorrectAnswer = table.Column<string>(nullable: true),
                    Question_Options = table.Column<string>(nullable: true),
                    Question_Hint = table.Column<string>(nullable: true),
                    Question_AnswerExplanatoryNote = table.Column<string>(nullable: true),
                    Question_Level = table.Column<int>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedQuestions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormAnswers");

            migrationBuilder.DropTable(
                name: "FormQuestionAnswers");

            migrationBuilder.DropTable(
                name: "FormQuestions");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "SharedQuestions");
        }
    }
}
