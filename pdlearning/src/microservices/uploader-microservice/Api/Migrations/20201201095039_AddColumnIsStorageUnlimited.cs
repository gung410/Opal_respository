using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Uploader.Migrations
{
    public partial class AddColumnIsStorageUnlimited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStorageUnlimited",
                table: "PersonalSpaces",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStorageUnlimited",
                table: "PersonalSpaces");
        }
    }
}
