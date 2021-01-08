using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class ImproveSpt22Indexes_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VersionTrackings_OriginalObjectId_CreatedDate",
                table: "VersionTrackings",
                columns: new[] { "OriginalObjectId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VersionTrackings_OriginalObjectId_MajorVersion_CreatedDate",
                table: "VersionTrackings",
                columns: new[] { "OriginalObjectId", "MajorVersion", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VersionTrackings_OriginalObjectId_CreatedDate",
                table: "VersionTrackings");

            migrationBuilder.DropIndex(
                name: "IX_VersionTrackings_OriginalObjectId_MajorVersion_CreatedDate",
                table: "VersionTrackings");
        }
    }
}
