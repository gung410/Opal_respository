using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.BrokenLink.Migrations
{
    public partial class UpdateIndexContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_OriginalObjectId_Module_CreatedDate",
                table: "BrokenLinkReports");

            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_ReportBy_Module_CreatedDate",
                table: "BrokenLinkReports");

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLinkReports_OriginalObjectId_Module_CreatedDate",
                table: "BrokenLinkReports",
                columns: new[] { "OriginalObjectId", "Module", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Id", "ObjectId", "Url", "Description", "ReportBy", "ReporterName", "ContentType", "ObjectTitle", "ObjectOwnerId", "ObjectOwnerName", "ObjectDetailUrl", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_BrokenLinkReports_ReportBy_ContentType_Module_CreatedDate",
                table: "BrokenLinkReports",
                columns: new[] { "ReportBy", "ContentType", "Module", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Id", "OriginalObjectId", "ObjectId", "Url", "ReporterName", "Description", "ObjectTitle", "ObjectOwnerId", "ObjectOwnerName", "ObjectDetailUrl", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_OriginalObjectId_Module_CreatedDate",
                table: "BrokenLinkReports");

            migrationBuilder.DropIndex(
                name: "IX_BrokenLinkReports_ReportBy_ContentType_Module_CreatedDate",
                table: "BrokenLinkReports");

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
    }
}
