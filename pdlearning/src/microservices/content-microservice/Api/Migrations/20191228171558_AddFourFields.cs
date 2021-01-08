using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddFourFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "DigitalContents",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "DigitalContents",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "DigitalContents",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfUse",
                table: "DigitalContents",
                maxLength: 4000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "TermsOfUse",
                table: "DigitalContents");
        }
    }
}
