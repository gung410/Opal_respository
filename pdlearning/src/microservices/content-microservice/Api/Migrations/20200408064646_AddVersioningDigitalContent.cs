using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddVersioningDigitalContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "DigitalContents",
                unicode: false,
                maxLength: 28,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)",
                oldUnicode: false,
                oldMaxLength: 21);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "DigitalContents",
                nullable: false,
                defaultValue: false);

            // Update Form set default value of IsArchived to False
            migrationBuilder.Sql("UPDATE DigitalContents SET IsArchived = 0");

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalObjectId",
                table: "DigitalContents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "DigitalContents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "VersionTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ObjectType = table.Column<int>(nullable: false),
                    OriginalObjectId = table.Column<Guid>(nullable: false),
                    ChangedByUserId = table.Column<Guid>(nullable: false),
                    RevertObjectId = table.Column<Guid>(nullable: false),
                    CanRollback = table.Column<bool>(nullable: false),
                    MajorVersion = table.Column<int>(nullable: false),
                    MinorVersion = table.Column<int>(nullable: false),
                    SchemaVersion = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionTrackings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VersionTrackings");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "OriginalObjectId",
                table: "DigitalContents");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DigitalContents");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "DigitalContents",
                type: "varchar(21)",
                unicode: false,
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 28);
        }
    }
}
