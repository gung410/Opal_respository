using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.BrokenLink.Migrations
{
    public partial class InitBrokenLinkDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrokenLinkReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(maxLength: 5000, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ReportBy = table.Column<Guid>(nullable: true),
                    IsSystemReport = table.Column<bool>(nullable: false),
                    OriginalObjectId = table.Column<Guid>(nullable: false),
                    ObjectTitle = table.Column<string>(maxLength: 5000, nullable: true),
                    ObjectOwnerId = table.Column<Guid>(nullable: false),
                    ObjectOwnerName = table.Column<string>(maxLength: 500, nullable: true),
                    ObjectDetailUrl = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokenLinkReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    Url = table.Column<string>(maxLength: 5000, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 18, nullable: false),
                    ObjectId = table.Column<Guid>(nullable: false),
                    OriginalObjectId = table.Column<Guid>(nullable: false),
                    ObjectTitle = table.Column<string>(maxLength: 5000, nullable: true),
                    ObjectOwnerName = table.Column<string>(maxLength: 500, nullable: true),
                    ObjectOwnerId = table.Column<Guid>(nullable: false),
                    ObjectDetailUrl = table.Column<string>(maxLength: 2000, nullable: true),
                    ScannedAt = table.Column<DateTime>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    RoutingKey = table.Column<string>(maxLength: 1000, nullable: true),
                    Exchange = table.Column<string>(maxLength: 1000, nullable: true),
                    MessageData = table.Column<string>(nullable: true),
                    SendTimes = table.Column<int>(nullable: false),
                    FailReason = table.Column<string>(nullable: true),
                    SourceIp = table.Column<string>(maxLength: 255, nullable: true),
                    UserId = table.Column<string>(maxLength: 255, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 19, nullable: false),
                    PreparedAt = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ReadyToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedDate_Status",
                table: "OutboxMessages",
                columns: new[] { "CreatedDate", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokenLinkReports");

            migrationBuilder.DropTable(
                name: "ExtractedUrls");

            migrationBuilder.DropTable(
                name: "OutboxMessages");
        }
    }
}
