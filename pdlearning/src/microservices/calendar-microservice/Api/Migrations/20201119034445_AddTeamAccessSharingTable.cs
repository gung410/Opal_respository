using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddTeamAccessSharingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamAccessSharings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAccessSharings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAccessSharings_OwnerId",
                table: "TeamAccessSharings",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAccessSharings_UserId",
                table: "TeamAccessSharings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAccessSharings");
        }
    }
}
