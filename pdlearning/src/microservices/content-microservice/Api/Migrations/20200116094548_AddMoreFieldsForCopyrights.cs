using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddMoreFieldsForCopyrights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcknowledgementAndCredit",
                table: "DigitalContents",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowDownload",
                table: "DigitalContents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowModification",
                table: "DigitalContents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowReusable",
                table: "DigitalContents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseTerritory",
                table: "DigitalContents",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "DigitalContents",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Ownership",
                table: "DigitalContents",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "DigitalContents",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "DigitalContents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcknowledgementAndCredit",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "IsAllowDownload",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "IsAllowModification",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "IsAllowReusable",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "LicenseTerritory",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "LicenseType",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "Ownership",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "DigitalContents");
        }
    }
}
