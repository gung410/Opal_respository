using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddFormRemindDateConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FormRemindDueDate",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemindBeforeDays",
                table: "Forms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FormParticipant",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "NotStarted",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "Incomplete");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormRemindDueDate",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "RemindBeforeDays",
                table: "Forms");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FormParticipant",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Incomplete",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "NotStarted");
        }
    }
}
