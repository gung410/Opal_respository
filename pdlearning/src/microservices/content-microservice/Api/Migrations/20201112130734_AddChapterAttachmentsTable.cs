using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddChapterAttachmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<Guid>(nullable: false), // ChapterId
                    FileLocation = table.Column<string>(maxLength: 5000, nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterAttachments_Chapters_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Chapters",
                        principalColumn: "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterAttachments");
        }
    }
}
