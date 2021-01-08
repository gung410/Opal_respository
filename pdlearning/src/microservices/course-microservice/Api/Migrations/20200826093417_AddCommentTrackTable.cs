using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCommentTrackTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentTrack",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    CommentId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentTrack", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentTrack_CreatedDate",
                table: "CommentTrack",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommentTrack_UserId_CreatedDate",
                table: "CommentTrack",
                columns: new[] { "UserId", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentTrack");
        }
    }
}
