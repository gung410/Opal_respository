using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarAutoscaler.Migrations
{
    public partial class InitBBBServerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BBBServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    IsProtection = table.Column<bool>(nullable: false),
                    PrivateIp = table.Column<string>(maxLength: 20, nullable: true),
                    InstanceId = table.Column<string>(maxLength: 100, nullable: true),
                    TargetGroupArn = table.Column<string>(maxLength: 255, nullable: true),
                    RuleArn = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BBBServers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BBBServers_PrivateIp",
                table: "BBBServers",
                column: "PrivateIp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BBBServers");
        }
    }
}
