using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddParticipantAssignmentTrackQuizAnswerAndParticipantAssignmentTrackQuizQuestionAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParticipantAssignmentTrackQuizAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    QuizAssignmentFormId = table.Column<Guid>(nullable: false),
                    Score = table.Column<float>(nullable: false),
                    ScorePercentage = table.Column<float>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantAssignmentTrackQuizAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantAssignmentTrackQuizAnswer_ParticipantAssignmentTrack_Id",
                        column: x => x.Id,
                        principalTable: "ParticipantAssignmentTrack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantAssignmentTrackQuizAnswer_QuizAssignmentForm_QuizAssignmentFormId",
                        column: x => x.QuizAssignmentFormId,
                        principalTable: "QuizAssignmentForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantAssignmentTrackQuizQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizAssignmentFormQuestionId = table.Column<Guid>(nullable: false),
                    AnswerValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManualScore = table.Column<float>(nullable: true),
                    ManualScoredBy = table.Column<Guid>(nullable: true),
                    Score = table.Column<float>(nullable: true),
                    ScoredBy = table.Column<Guid>(nullable: true),
                    SubmittedDate = table.Column<DateTime>(nullable: true),
                    QuizAnswerId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantAssignmentTrackQuizQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantAssignmentTrackQuizQuestionAnswer_ParticipantAssignmentTrackQuizAnswer_QuizAnswerId",
                        column: x => x.QuizAnswerId,
                        principalTable: "ParticipantAssignmentTrackQuizAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizAnswer_IsDeleted",
                table: "ParticipantAssignmentTrackQuizAnswer",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizAnswer_QuizAssignmentFormId",
                table: "ParticipantAssignmentTrackQuizAnswer",
                column: "QuizAssignmentFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_IsDeleted",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_ManualScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "ManualScoredBy");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_QuizAnswerId",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "QuizAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_QuizAssignmentFormQuestionId",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "QuizAssignmentFormQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_ScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "ScoredBy");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_SubmittedDate",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "SubmittedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantAssignmentTrackQuizQuestionAnswer");

            migrationBuilder.DropTable(
                name: "ParticipantAssignmentTrackQuizAnswer");
        }
    }
}
