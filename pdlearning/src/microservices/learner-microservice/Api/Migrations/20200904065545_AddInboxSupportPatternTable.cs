using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddInboxSupportPatternTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    MessageData = table.Column<string>(nullable: true),
                    MessageId = table.Column<Guid>(nullable: false),
                    MessageCreatedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(unicode: false, maxLength: 19, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ReadyToDelete = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_Status",
                table: "InboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_CreatedDate_Status",
                table: "InboxMessages",
                columns: new[] { "CreatedDate", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages");
        }
    }
}
