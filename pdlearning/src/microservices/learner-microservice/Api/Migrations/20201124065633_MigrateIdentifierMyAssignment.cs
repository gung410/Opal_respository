using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class MigrateIdentifierMyAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE MyAssignments SET Id = ParticipantAssignmentTrackId;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
