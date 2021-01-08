using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Migrations
{
    public partial class AddStartEndDateForParticipantAssignmentTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ParticipantAssignmentTrack",
                nullable: false,
                defaultValue: Clock.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ParticipantAssignmentTrack",
                nullable: false,
                defaultValue: Clock.Now);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrack_EndDate",
                table: "ParticipantAssignmentTrack",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrack_StartDate",
                table: "ParticipantAssignmentTrack",
                column: "StartDate");

            migrationBuilder.Sql(@"
                UPDATE ParticipantAssignmentTrack
                SET ParticipantAssignmentTrack.StartDate = Assignment.StartDate, ParticipantAssignmentTrack.EndDate = Assignment.StartDate
                FROM ParticipantAssignmentTrack join Assignment on ParticipantAssignmentTrack.AssignmentId = Assignment.Id");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_EndDate",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_StartDate",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Assignment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParticipantAssignmentTrack_EndDate",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantAssignmentTrack_StartDate",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ParticipantAssignmentTrack");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Assignment",
                type: "datetime2",
                nullable: false,
                defaultValue: Clock.Now);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Assignment",
                type: "datetime2",
                nullable: false,
                defaultValue: Clock.Now);

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_EndDate",
                table: "Assignment",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_StartDate",
                table: "Assignment",
                column: "StartDate");
        }
    }
}
