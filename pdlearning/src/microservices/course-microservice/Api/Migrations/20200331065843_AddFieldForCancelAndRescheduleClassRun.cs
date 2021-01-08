using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddFieldForCancelAndRescheduleClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ClassRun",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Unpublished",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "Unpublished");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "ClassRun",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "CancellationStatus",
                table: "ClassRun",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RescheduleEndDateTime",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RescheduleStartDateTime",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RescheduleStatus",
                table: "ClassRun",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationStatus",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "RescheduleEndDateTime",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "RescheduleStartDateTime",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "RescheduleStatus",
                table: "ClassRun");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ClassRun",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Unpublished",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "Unpublished");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "ClassRun",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
