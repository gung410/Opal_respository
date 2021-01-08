using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFieldsForCourseAndRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Registration",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "PendingConfirmation",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationType",
                table: "Registration",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Application",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Registration",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanningArchiveDate",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanningPublishDate",
                table: "Course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "PlanningArchiveDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PlanningPublishDate",
                table: "Course");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Registration",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "PendingConfirmation");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationType",
                table: "Registration",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "Application");
        }
    }
}
