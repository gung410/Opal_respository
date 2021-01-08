using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class RemoveDefaultValueStatusEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                unicode: false,
                maxLength: 19,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(19)",
                oldUnicode: false,
                oldMaxLength: 19,
                oldDefaultValue: "Opening");

            migrationBuilder.Sql(@"UPDATE Events
                                    SET Status = 'Opening'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "varchar(19)",
                unicode: false,
                maxLength: 19,
                nullable: false,
                defaultValue: "Opening",
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 19);
        }
    }
}
