using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddIndexCommunityMembershipsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId_UserId",
                table: "CommunityMemberships",
                columns: new[] { "CommunityId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommunityMemberships_CommunityId_UserId",
                table: "CommunityMemberships");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId");
        }
    }
}
