using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddIndexCalendarUserPaoAao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_AlternativeApprovalOfficerId",
                table: "Users",
                column: "AlternativeApprovalOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrimaryApprovalOfficerId",
                table: "Users",
                column: "PrimaryApprovalOfficerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AlternativeApprovalOfficerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrimaryApprovalOfficerId",
                table: "Users");
        }
    }
}
