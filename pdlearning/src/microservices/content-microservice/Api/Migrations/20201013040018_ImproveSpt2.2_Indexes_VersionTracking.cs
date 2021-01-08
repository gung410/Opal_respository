using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class ImproveSpt22_Indexes_VersionTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_CreatedDate_Status",
                table: "OutboxMessages");

            migrationBuilder.CreateIndex(
                name: "IX_VersionTrackings_OriginalObjectId_CreatedDate",
                table: "VersionTrackings",
                columns: new[] { "OriginalObjectId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VersionTrackings_OriginalObjectId_MajorVersion_CreatedDate",
                table: "VersionTrackings",
                columns: new[] { "OriginalObjectId", "MajorVersion", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status_CreatedDate",
                table: "OutboxMessages",
                columns: new[] { "Status", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VersionTrackings_OriginalObjectId_CreatedDate",
                table: "VersionTrackings");

            migrationBuilder.DropIndex(
                name: "IX_VersionTrackings_OriginalObjectId_MajorVersion_CreatedDate",
                table: "VersionTrackings");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Status_CreatedDate",
                table: "OutboxMessages");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedDate_Status",
                table: "OutboxMessages",
                columns: new[] { "CreatedDate", "Status" });
        }
    }
}
