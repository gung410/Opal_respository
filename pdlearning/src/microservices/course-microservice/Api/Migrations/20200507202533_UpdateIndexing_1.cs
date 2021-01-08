using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateIndexing_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Course_AlternativeApprovingOfficerId",
                table: "Course",
                column: "AlternativeApprovingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_ECertificateTemplateId",
                table: "Course",
                column: "ECertificateTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_FirstAdministratorId",
                table: "Course",
                column: "FirstAdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_MOEOfficerId",
                table: "Course",
                column: "MOEOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_PDAreaThemeId",
                table: "Course",
                column: "PDAreaThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_PrimaryApprovingOfficerId",
                table: "Course",
                column: "PrimaryApprovingOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_SecondAdministratorId",
                table: "Course",
                column: "SecondAdministratorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_AlternativeApprovingOfficerId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ECertificateTemplateId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_FirstAdministratorId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_MOEOfficerId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PDAreaThemeId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PrimaryApprovingOfficerId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_SecondAdministratorId",
                table: "Course");
        }
    }
}
