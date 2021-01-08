using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RestrictUserToHaveMultipleRecordsInSomeCases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_UserId_ItemId",
                table: "UserBookmarks",
                columns: new[] { "UserId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_UserId_CourseId",
                table: "CourseReviews",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBookmarks_UserId_ItemId",
                table: "UserBookmarks");

            migrationBuilder.DropIndex(
                name: "IX_CourseReviews_UserId_CourseId",
                table: "CourseReviews");
        }
    }
}
