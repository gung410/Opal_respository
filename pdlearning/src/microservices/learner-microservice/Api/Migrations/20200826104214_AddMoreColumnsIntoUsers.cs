using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddMoreColumnsIntoUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Users",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "Internal");

            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeApprovingOfficerId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryApprovingOfficerId",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "New");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AlternativeApprovingOfficerId",
                table: "Users",
                column: "AlternativeApprovingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentID",
                table: "Users",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserID",
                table: "Users",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrimaryApprovingOfficerId",
                table: "Users",
                column: "PrimaryApprovingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AlternativeApprovingOfficerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrimaryApprovingOfficerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AlternativeApprovingOfficerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrimaryApprovingOfficerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }
    }
}
