using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarAutoscaler.Migrations
{
    public partial class UpdateParticipantCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "BBBServers");

            migrationBuilder.AddColumn<Guid>(
                name: "BBBServerId",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ParticipantCount",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BBBServers",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(22)",
                oldUnicode: false,
                oldMaxLength: 22);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BBBServerId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "ParticipantCount",
                table: "Meetings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BBBServers",
                type: "varchar(22)",
                unicode: false,
                maxLength: 22,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldUnicode: false,
                oldMaxLength: 15);

            migrationBuilder.AddColumn<Guid>(
                name: "MeetingId",
                table: "BBBServers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
