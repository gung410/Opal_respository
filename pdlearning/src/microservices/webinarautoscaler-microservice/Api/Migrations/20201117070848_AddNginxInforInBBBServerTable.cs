using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarAutoscaler.Migrations
{
    public partial class AddNginxInforInBBBServerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BBBServers",
                type: "varchar(22)",
                unicode: false,
                maxLength: 22,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

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

            migrationBuilder.CreateIndex(
                name: "IX_BBBServers_InstanceId",
                table: "BBBServers",
                column: "InstanceId",
                unique: true,
                filter: "[InstanceId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BBBServers_InstanceId",
                table: "BBBServers");

            migrationBuilder.DropColumn(
                name: "NginxInstanceId",
                table: "BBBServers");

            migrationBuilder.DropColumn(
                name: "NginxPrivateIp",
                table: "BBBServers");

            migrationBuilder.DropColumn(
                name: "NginxPublicIp",
                table: "BBBServers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BBBServers",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(22)",
                oldUnicode: false,
                oldMaxLength: 22);
        }
    }
}
