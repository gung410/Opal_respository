using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddBrokenLinkTables : Migration
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
                    Url = table.Column<string>(maxLength: 5000, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ReportBy = table.Column<Guid>(nullable: false),
                    ObjectId = table.Column<Guid>(nullable: false),
                    OriginalObjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokenLinkReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractionUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    Url = table.Column<string>(maxLength: 5000, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 18, nullable: false),
                    ObjectId = table.Column<Guid>(nullable: false),
                    OriginalObjectId = table.Column<Guid>(nullable: false),
                    ScannedAt = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractionUrls", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokenLinkReports");

            migrationBuilder.DropTable(
                name: "ExtractionUrls");
        }
    }
}
