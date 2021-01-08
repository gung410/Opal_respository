using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class Remove_ParticipantAssignmentTrackQuizQuestionAnswer_ScoredBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_ScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer");

            migrationBuilder.DropColumn(
                name: "ScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAssignmentTrackQuizQuestionAnswer_ScoredBy",
                table: "ParticipantAssignmentTrackQuizQuestionAnswer",
                column: "ScoredBy");
        }
    }
}
