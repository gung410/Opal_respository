using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddFourFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TermsOfUse",
                table: "Course",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "Course",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Course",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Course",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Course");

            migrationBuilder.AlterColumn<string>(
                name: "TermsOfUse",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000,
                oldNullable: true);
        }
    }
}
