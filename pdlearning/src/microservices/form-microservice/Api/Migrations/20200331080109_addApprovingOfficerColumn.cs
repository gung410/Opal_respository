using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddApprovingOfficerColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeApprovingOfficerId",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryApprovingOfficerId",
                table: "Forms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternativeApprovingOfficerId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "PrimaryApprovingOfficerId",
                table: "Forms");
        }
    }
}
