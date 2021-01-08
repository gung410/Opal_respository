using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddFormParticipantOriginId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FormParticipant",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Incomplete",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "NotStarted");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStarted",
                table: "FormParticipant",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FormOriginalObjectId",
                table: "FormParticipant",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormOriginalObjectId",
                table: "FormParticipant");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FormParticipant",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "NotStarted",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "Incomplete");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStarted",
                table: "FormParticipant",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);
        }
    }
}
