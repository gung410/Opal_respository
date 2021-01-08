using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Webinar.Migrations
{
    public partial class AddPrecordInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreRecordURL",
                table: "Meetings",
                newName: "PreRecordPath");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreRecordId",
                table: "Meetings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreRecordPath",
                table: "Meetings",
                newName: "PreRecordURL");

            migrationBuilder.AlterColumn<string>(
                name: "PreRecordId",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
