using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarAutoscaler.Migrations
{
    public partial class UpdatemeetingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedDate",
                table: "Meetings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Meetings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedDate",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Meetings");
        }
    }
}
