using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Webinar.Migrations
{
    public partial class AddPrePrecordingUrlColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreRecordURL",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreRecordURL",
                table: "Meetings");
        }
    }
}
