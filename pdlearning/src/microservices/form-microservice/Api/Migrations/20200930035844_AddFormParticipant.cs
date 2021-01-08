using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddFormParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStandalone",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandaloneMode",
                table: "Forms",
                type: "varchar(30)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    FormId = table.Column<Guid>(nullable: false),
                    AssignedDate = table.Column<DateTime>(nullable: false),
                    SubmittedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 30, nullable: false, defaultValue: "NotStarted"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormParticipant", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormParticipant");

            migrationBuilder.DropColumn(
                name: "IsStandalone",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "StandaloneMode",
                table: "Forms");
        }
    }
}
