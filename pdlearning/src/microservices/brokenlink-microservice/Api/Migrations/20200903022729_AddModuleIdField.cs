using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.BrokenLink.Migrations
{
    public partial class AddModuleIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Module",
                table: "ExtractedUrls",
                unicode: false,
                maxLength: 19,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<string>(
                name: "Module",
                table: "BrokenLinkReports",
                unicode: false,
                maxLength: 19,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLinkReports_OriginalObjectId_Module_CreatedDate",
                table: "BrokenLinkReports",
                columns: new[] { "OriginalObjectId", "Module", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Id", "ObjectId", "Url", "Description", "ReportBy", "ObjectTitle", "ObjectOwnerId", "ObjectOwnerName", "ObjectDetailUrl", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLinkReports_ReportBy_Module_CreatedDate",
                table: "BrokenLinkReports",
                columns: new[] { "ReportBy", "Module", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Id", "OriginalObjectId", "ObjectId", "Url", "Description", "ObjectTitle", "ObjectOwnerId", "ObjectOwnerName", "ObjectDetailUrl", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_OriginalObjectId_Module_CreatedDate",
                table: "BrokenLinkReports");

            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_ReportBy_Module_CreatedDate",
                table: "BrokenLinkReports");

            migrationBuilder.DropColumn(
                name: "Module",
                table: "ExtractedUrls");

            migrationBuilder.DropColumn(
                name: "Module",
                table: "BrokenLinkReports");
        }
    }
}
