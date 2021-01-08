using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarVideoConverter.Migrations
{
    public partial class AddConvertingTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConvertingTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternalMeetingId = table.Column<string>(type: "nvarchar(54)", maxLength: 54, nullable: false),
                    Status = table.Column<string>(type: "varchar(21)", unicode: false, maxLength: 21, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertingTrackings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConvertingTrackings_InternalMeetingId",
                table: "ConvertingTrackings",
                column: "InternalMeetingId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertingTrackings");
        }
    }
}
