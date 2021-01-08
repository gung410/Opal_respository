using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddEventCommunityEventTypeDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CommunityEventType",
                table: "Events",
                unicode: false,
                maxLength: 17,
                nullable: true,
                defaultValue: "None",
                oldClrType: typeof(string),
                oldType: "varchar(17)",
                oldUnicode: false,
                oldMaxLength: 17,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CommunityEventType",
                table: "Events",
                type: "varchar(17)",
                unicode: false,
                maxLength: 17,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 17,
                oldNullable: true,
                oldDefaultValue: "None");
        }
    }
}
