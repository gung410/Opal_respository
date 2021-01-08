using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ItemId_Rate_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "ItemId", "Rate", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserReviews_ItemId_Rate_IsDeleted_CreatedDate",
                table: "UserReviews");
        }
    }
}
