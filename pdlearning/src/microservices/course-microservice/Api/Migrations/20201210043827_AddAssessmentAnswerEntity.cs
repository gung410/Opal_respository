using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddAssessmentAnswerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAssigedAssessorOnce",
                table: "ParticipantAssignmentTrack",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AssessmentId",
                table: "Assignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberAutoAssessor",
                table: "Assignment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssessmentAnswer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantAssignmentTrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriteriaAnswers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAnswer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_AssessmentId_IsDeleted_CreatedDate",
                table: "AssessmentAnswer",
                columns: new[] { "AssessmentId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_CreatedBy_IsDeleted_CreatedDate",
                table: "AssessmentAnswer",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_IsDeleted",
                table: "AssessmentAnswer",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_IsDeleted_CreatedDate",
                table: "AssessmentAnswer",
                columns: new[] { "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_ParticipantAssignmentTrackId_IsDeleted_CreatedDate",
                table: "AssessmentAnswer",
                columns: new[] { "ParticipantAssignmentTrackId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAnswer_UserId_IsDeleted_CreatedDate",
                table: "AssessmentAnswer",
                columns: new[] { "UserId", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentAnswer");

            migrationBuilder.DropColumn(
                name: "IsAutoAssigedAssessorOnce",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.DropColumn(
                name: "AssessmentId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "NumberAutoAssessor",
                table: "Assignment");
        }
    }
}
