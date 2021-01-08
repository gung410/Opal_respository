using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddIsAutoAssignedOnceAndIsAssignedAssessmentManuallyOnce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoAssignedOnce",
                table: "ParticipantAssignmentTrack",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssignedAssessmentManuallyOnce",
                table: "ParticipantAssignmentTrack",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoAssignedOnce",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.DropColumn(
                name: "IsAssignedAssessmentManuallyOnce",
                table: "ParticipantAssignmentTrack");
        }
    }
}
