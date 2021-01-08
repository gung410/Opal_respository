using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class RefactorTableEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunityEventType",
                table: "Events",
                unicode: false,
                maxLength: 17,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "CommunityMemberships",
                unicode: false,
                maxLength: 19,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityEventType",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "CommunityMemberships",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 19);
        }
    }
}
