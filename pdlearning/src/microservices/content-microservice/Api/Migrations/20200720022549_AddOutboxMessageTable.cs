using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddOutboxMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    RoutingKey = table.Column<string>(maxLength: 1000, nullable: true),
                    MessageData = table.Column<string>(nullable: true),
                    SendTimes = table.Column<int>(nullable: false),
                    FailReason = table.Column<string>(nullable: true),
                    SourceIp = table.Column<string>(maxLength: 255, nullable: true),
                    UserId = table.Column<string>(maxLength: 255, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 19, nullable: false),
                    PreparedAt = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ReadyToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedDate_Status",
                table: "OutboxMessages",
                columns: new[] { "CreatedDate", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");
        }
    }
}
