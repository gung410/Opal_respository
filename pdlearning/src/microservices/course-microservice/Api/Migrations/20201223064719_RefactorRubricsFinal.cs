using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RefactorRubricsFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignmentAssessmentConfig_ScoreMode",
                table: "Assignment",
                newName: "AssessmentConfig_ScoreMode");

            migrationBuilder.RenameColumn(
                name: "AssignmentAssessmentConfig_NumberAutoAssessor",
                table: "Assignment",
                newName: "AssessmentConfig_NumberAutoAssessor");

            migrationBuilder.RenameColumn(
                name: "AssignmentAssessmentConfig_AssessmentId",
                table: "Assignment",
                newName: "AssessmentConfig_AssessmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssessmentConfig_ScoreMode",
                table: "Assignment",
                newName: "AssignmentAssessmentConfig_ScoreMode");

            migrationBuilder.RenameColumn(
                name: "AssessmentConfig_NumberAutoAssessor",
                table: "Assignment",
                newName: "AssignmentAssessmentConfig_NumberAutoAssessor");

            migrationBuilder.RenameColumn(
                name: "AssessmentConfig_AssessmentId",
                table: "Assignment",
                newName: "AssignmentAssessmentConfig_AssessmentId");
        }
    }
}
