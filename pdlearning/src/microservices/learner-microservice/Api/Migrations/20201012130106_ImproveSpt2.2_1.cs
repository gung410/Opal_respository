using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LecturesInMyCourse_CreatedBy_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturesInMyCourse_LectureId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse",
                columns: new[] { "LectureId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturesInMyCourse_MyCourseId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse",
                columns: new[] { "MyCourseId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturesInMyCourse_Status_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse",
                columns: new[] { "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturesInMyCourse_UserId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse",
                columns: new[] { "UserId", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LecturesInMyCourse_CreatedBy_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse");

            migrationBuilder.DropIndex(
                name: "IX_LecturesInMyCourse_LectureId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse");

            migrationBuilder.DropIndex(
                name: "IX_LecturesInMyCourse_MyCourseId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse");

            migrationBuilder.DropIndex(
                name: "IX_LecturesInMyCourse_Status_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse");

            migrationBuilder.DropIndex(
                name: "IX_LecturesInMyCourse_UserId_IsDeleted_CreatedDate",
                table: "LecturesInMyCourse");
        }
    }
}
