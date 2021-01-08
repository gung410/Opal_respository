using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddStatusColumnForParticipantAssignmentTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ParticipantAssignmentTrack",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "NotStarted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ParticipantAssignmentTrack");
        }
    }
}
