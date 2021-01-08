using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Uploader.Migrations
{
    public partial class RemoveFileGroupInPersonalFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileGroup",
                table: "PersonalFiles");

            migrationBuilder.AlterColumn<long>(
                name: "TotalUsed",
                table: "PersonalSpaces",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "TotalSpace",
                table: "PersonalSpaces",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "PersonalFiles",
                type: "varchar(30)",
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalUsed",
                table: "PersonalSpaces",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "TotalSpace",
                table: "PersonalSpaces",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "PersonalFiles",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)");

            migrationBuilder.AddColumn<string>(
                name: "FileGroup",
                table: "PersonalFiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
