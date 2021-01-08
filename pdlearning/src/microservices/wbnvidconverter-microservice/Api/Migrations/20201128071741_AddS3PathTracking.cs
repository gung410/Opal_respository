using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.WebinarVideoConverter.Migrations
{
    public partial class AddS3PathTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "S3Path",
                table: "ConvertingTrackings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "S3Path",
                table: "ConvertingTrackings");
        }
    }
}
