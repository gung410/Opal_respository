using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddApprovalOfficerUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeApprovalOfficerId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryApprovalOfficerId",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternativeApprovalOfficerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrimaryApprovalOfficerId",
                table: "Users");
        }
    }
}
