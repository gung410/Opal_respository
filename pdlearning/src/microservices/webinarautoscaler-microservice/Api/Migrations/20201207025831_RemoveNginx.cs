using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarAutoscaler.Migrations
{
    public partial class RemoveNginx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NginxInstanceId",
                table: "BBBServers");

            migrationBuilder.DropColumn(
                name: "NginxPrivateIp",
                table: "BBBServers");

            migrationBuilder.DropColumn(
                name: "NginxPublicIp",
                table: "BBBServers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NginxInstanceId",
                table: "BBBServers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NginxPrivateIp",
                table: "BBBServers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NginxPublicIp",
                table: "BBBServers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
