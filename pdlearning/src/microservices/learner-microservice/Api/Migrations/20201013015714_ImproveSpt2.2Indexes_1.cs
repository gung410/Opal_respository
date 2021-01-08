using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22Indexes_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status_FirstName",
                table: "Users",
                columns: new[] { "Status", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status_LastName",
                table: "Users",
                columns: new[] { "Status", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Status_FirstName",
                table: "Users",
                columns: new[] { "Email", "Status", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Status_LastName",
                table: "Users",
                columns: new[] { "Email", "Status", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status_LastName_FirstName",
                table: "Users",
                columns: new[] { "Status", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowing_UserId_FollowingUserId_CreatedDate",
                table: "UserFollowing",
                columns: new[] { "UserId", "FollowingUserId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_UserId_ItemType_CreatedDate",
                table: "UserBookmarks",
                columns: new[] { "UserId", "ItemType", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MyClassRun_CourseId_UserId_Status_CreatedDate",
                table: "MyClassRun",
                columns: new[] { "CourseId", "UserId", "Status", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FirstName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LastName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status_FirstName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status_LastName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Status_FirstName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Status_LastName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status_LastName_FirstName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserFollowing_UserId_FollowingUserId_CreatedDate",
                table: "UserFollowing");

            migrationBuilder.DropIndex(
                name: "IX_UserBookmarks_UserId_ItemType_CreatedDate",
                table: "UserBookmarks");

            migrationBuilder.DropIndex(
                name: "IX_MyClassRun_CourseId_UserId_Status_CreatedDate",
                table: "MyClassRun");
        }
    }
}
