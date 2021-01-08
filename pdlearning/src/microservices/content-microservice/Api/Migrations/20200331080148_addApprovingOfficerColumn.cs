using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddApprovingOfficerColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeApprovingOfficerId",
                table: "DigitalContents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryApprovingOfficerId",
                table: "DigitalContents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternativeApprovingOfficerId",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "PrimaryApprovingOfficerId",
                table: "DigitalContents");
        }
    }
}
