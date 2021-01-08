using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class ChangeEventType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityEventType",
                table: "Events");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunityEventType",
                table: "Events",
                type: "varchar(17)",
                unicode: false,
                maxLength: 17,
                nullable: true,
                defaultValue: "None");
        }
    }
}
