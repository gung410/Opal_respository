using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class IX_UserPersonalEvents_UserId_IncludeProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPersonalEvents_UserId",
                table: "UserPersonalEvents");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonalEvents_UserId",
                table: "UserPersonalEvents",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "EventId", "IsAccepted" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPersonalEvents_UserId",
                table: "UserPersonalEvents");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonalEvents_UserId",
                table: "UserPersonalEvents",
                column: "UserId");
        }
    }
}
