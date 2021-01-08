using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddCommunityStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Events",
                type: "varchar(26)",
                unicode: false,
                maxLength: 26,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(24)",
                oldUnicode: false,
                oldMaxLength: 24);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Communities",
                type: "varchar(18)",
                unicode: false,
                maxLength: 18,
                nullable: false,
                defaultValue: string.Empty); // Replace "" by string.Empty due to convention.

            migrationBuilder.CreateIndex(
                name: "IX_Communities_Status",
                table: "Communities",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_Status",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Communities");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Events",
                type: "varchar(24)",
                unicode: false,
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(26)",
                oldUnicode: false,
                oldMaxLength: 26);
        }
    }
}
