using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class ChangeResourceIdToCourseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResourceId",
                table: "FormAnswers",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_FormAnswers_ResourceId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                newName: "IX_FormAnswers_CourseId_IsDeleted_CreatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "FormAnswers",
                newName: "ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_FormAnswers_CourseId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                newName: "IX_FormAnswers_ResourceId_IsDeleted_CreatedDate");
        }
    }
}
