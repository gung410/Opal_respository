using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class UpdateLearningTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrackingType",
                table: "LearningTrackings",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "DigitalContent",
                oldClrType: typeof(string),
                oldType: "varchar(30)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingAction",
                table: "LearningTrackings",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "View",
                oldClrType: typeof(string),
                oldType: "varchar(30)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrackingType",
                table: "LearningTrackings",
                type: "varchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "DigitalContent");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingAction",
                table: "LearningTrackings",
                type: "varchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldDefaultValue: "View");
        }
    }
}
