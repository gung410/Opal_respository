using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddClassRunTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "varchar(18)",
                unicode: false,
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(19)",
                oldUnicode: false,
                oldMaxLength: 19);

            migrationBuilder.CreateTable(
                name: "ClassRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRuns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassRuns");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "varchar(19)",
                unicode: false,
                maxLength: 19,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(18)",
                oldUnicode: false,
                oldMaxLength: 18);
        }
    }
}
