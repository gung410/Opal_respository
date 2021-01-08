using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarVideoConverter.Migrations
{
    public partial class AddConvertingTrackingConcurrencyCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "ConvertingTrackings",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ConvertingTrackings");
        }
    }
}
