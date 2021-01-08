using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddColumnIsAutoPublishAndAutoPublishDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AutoPublishDate",
                table: "DigitalContents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoPublish",
                table: "DigitalContents",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoPublishDate",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "IsAutoPublish",
                table: "DigitalContents");
        }
    }
}
