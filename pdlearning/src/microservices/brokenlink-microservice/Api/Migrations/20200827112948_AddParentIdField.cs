using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.BrokenLink.Migrations
{
    public partial class AddParentIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalObjectId",
                table: "ExtractedUrls",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "ExtractedUrls",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalObjectId",
                table: "BrokenLinkReports",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "BrokenLinkReports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ExtractedUrls");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BrokenLinkReports");

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalObjectId",
                table: "ExtractedUrls",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalObjectId",
                table: "BrokenLinkReports",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
