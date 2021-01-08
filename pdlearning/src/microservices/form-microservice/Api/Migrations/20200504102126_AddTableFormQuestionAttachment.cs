using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddTableFormQuestionAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormQuestionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FormQuestionId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: true),
                    FileType = table.Column<string>(maxLength: 100, nullable: true),
                    FileExtension = table.Column<string>(maxLength: 10, nullable: true),
                    FileLocation = table.Column<string>(maxLength: 1000, nullable: true),
                    ExternalId = table.Column<string>(maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestionAttachments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormQuestionAttachments");
        }
    }
}
