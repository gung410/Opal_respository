using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.BrokenLink.Migrations
{
    public partial class AddColumnContentTypeAndReporterName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ExtractedUrls",
                unicode: false,
                maxLength: 25,
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "BrokenLinkReports",
                unicode: false,
                maxLength: 25,
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "BrokenLinkReports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ExtractedUrls");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "BrokenLinkReports");

            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "BrokenLinkReports");
        }
    }
}
