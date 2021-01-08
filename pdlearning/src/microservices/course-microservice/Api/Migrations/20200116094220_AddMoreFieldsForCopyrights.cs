using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddMoreFieldsForCopyrights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowDownload",
                table: "Course",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AddColumn<string>(
                name: "AcknowledgementAndCredit",
                table: "Course",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowModification",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowReusable",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseTerritory",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Ownership",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Course",
                maxLength: 4000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcknowledgementAndCredit",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAllowModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAllowReusable",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LicenseTerritory",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LicenseType",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Ownership",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Course");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowDownload",
                table: "Course",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
