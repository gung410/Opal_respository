using System;
using Microservice.Content.Domain.ValueObject;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddCreatedByColumnIntoChapterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "Chapters",
                nullable: false,
                defaultValue: VideoSourceType.DigitalContent);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Chapters",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Chapters",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Chapters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Chapters");
        }
    }
}
