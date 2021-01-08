using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AnnouncementRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "AnnouncementTemplate",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "AnnouncementTemplate",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Announcement",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Announcement",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_ClassrunId_IsDeleted_CreatedDate",
                table: "Announcement",
                columns: new[] { "ClassrunId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_CourseId_IsDeleted_CreatedDate",
                table: "Announcement",
                columns: new[] { "CourseId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Announcement_ScheduleDate_IsDeleted_CreatedDate",
                table: "Announcement",
                columns: new[] { "ScheduleDate", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcement_ClassrunId_IsDeleted_CreatedDate",
                table: "Announcement");

            migrationBuilder.DropIndex(
                name: "IX_Announcement_CourseId_IsDeleted_CreatedDate",
                table: "Announcement");

            migrationBuilder.DropIndex(
                name: "IX_Announcement_ScheduleDate_IsDeleted_CreatedDate",
                table: "Announcement");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "AnnouncementTemplate");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Announcement");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Announcement");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "AnnouncementTemplate",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
