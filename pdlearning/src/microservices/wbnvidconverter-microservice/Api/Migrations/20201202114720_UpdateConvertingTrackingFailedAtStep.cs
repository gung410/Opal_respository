using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarVideoConverter.Migrations
{
    public partial class UpdateConvertingTrackingFailedAtStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailedAtStep",
                table: "ConvertingTrackings",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: FailStep.None.ToString());

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "ConvertingTrackings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAtStep",
                table: "ConvertingTrackings");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "ConvertingTrackings");
        }
    }
}
