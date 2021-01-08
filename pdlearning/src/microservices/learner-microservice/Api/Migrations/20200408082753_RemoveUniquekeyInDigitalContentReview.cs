using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RemoveUniquekeyInDigitalContentReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DigitalContentReview_UserId_DigitalContentId",
                table: "DigitalContentReview");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DigitalContentReview_UserId_DigitalContentId",
                table: "DigitalContentReview",
                columns: new[] { "UserId", "DigitalContentId" },
                unique: true);
        }
    }
}
