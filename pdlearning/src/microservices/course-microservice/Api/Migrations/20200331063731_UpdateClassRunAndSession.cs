using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateClassRunAndSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Session",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Session",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Session",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionTitle",
                table: "Session",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "Session",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "SessionTitle",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "Session");
        }
    }
}
