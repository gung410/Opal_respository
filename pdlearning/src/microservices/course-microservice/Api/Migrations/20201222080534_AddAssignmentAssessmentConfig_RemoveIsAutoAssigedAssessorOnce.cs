using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddAssignmentAssessmentConfig_RemoveIsAutoAssigedAssessorOnce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoAssigedAssessorOnce",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.RenameColumn(
                name: "NumberAutoAssessor",
                table: "Assignment",
                newName: "AssignmentAssessmentConfig_NumberAutoAssessor");

            migrationBuilder.RenameColumn(
                name: "AssessmentId",
                table: "Assignment",
                newName: "AssignmentAssessmentConfig_AssessmentId");

            migrationBuilder.AlterColumn<int>(
                name: "AssignmentAssessmentConfig_NumberAutoAssessor",
                table: "Assignment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AssignmentAssessmentConfig_ScoreMode",
                table: "Assignment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentAssessmentConfig_ScoreMode",
                table: "Assignment");

            migrationBuilder.RenameColumn(
                name: "AssignmentAssessmentConfig_NumberAutoAssessor",
                table: "Assignment",
                newName: "NumberAutoAssessor");

            migrationBuilder.RenameColumn(
                name: "AssignmentAssessmentConfig_AssessmentId",
                table: "Assignment",
                newName: "AssessmentId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAssigedAssessorOnce",
                table: "ParticipantAssignmentTrack",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "NumberAutoAssessor",
                table: "Assignment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
