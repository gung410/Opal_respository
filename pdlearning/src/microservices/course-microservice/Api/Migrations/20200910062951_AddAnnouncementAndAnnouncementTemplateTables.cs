using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddAnnouncementAndAnnouncementTemplateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcement",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ScheduleDate = table.Column<DateTime>(nullable: true),
                    Participants = table.Column<string>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    ClassrunId = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FullTextSearch = table.Column<string>(maxLength: 2100, nullable: true),
                    FullTextSearchKey = table.Column<string>(unicode: false, maxLength: 100, nullable: false, defaultValue: string.Empty)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementTemplate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_CreatedDate",
                table: "Announcement",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_IsDeleted",
                table: "Announcement",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_Status_IsDeleted_CreatedDate",
                table: "Announcement",
                columns: new[] { "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_Title_IsDeleted_CreatedDate",
                table: "Announcement",
                columns: new[] { "Title", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTemplate_CreatedDate",
                table: "AnnouncementTemplate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTemplate_FullTextSearchKey",
                table: "AnnouncementTemplate",
                column: "FullTextSearchKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTemplate_IsDeleted",
                table: "AnnouncementTemplate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTemplate_CreatedBy_IsDeleted_CreatedDate",
                table: "AnnouncementTemplate",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementTemplate_Title_IsDeleted_CreatedDate",
                table: "AnnouncementTemplate",
                columns: new[] { "Title", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcement");

            migrationBuilder.DropTable(
                name: "AnnouncementTemplate");
        }
    }
}
