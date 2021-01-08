using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class ImproveSpt22_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FormSections_CreatedBy_IsDeleted_CreatedDate",
                table: "FormSections",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormSections_FormId_IsDeleted_CreatedDate",
                table: "FormSections",
                columns: new[] { "FormId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedBy_IsDeleted_CreatedDate",
                table: "Forms",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestions_CreatedBy_IsDeleted_CreatedDate",
                table: "FormQuestions",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestions_FormId_IsDeleted_CreatedDate",
                table: "FormQuestions",
                columns: new[] { "FormId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestionAnswers_CreatedBy_CreatedDate",
                table: "FormQuestionAnswers",
                columns: new[] { "CreatedBy", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestionAnswers_FormAnswerId_CreatedDate",
                table: "FormQuestionAnswers",
                columns: new[] { "FormAnswerId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestionAnswers_ScoredBy_CreatedDate",
                table: "FormQuestionAnswers",
                columns: new[] { "ScoredBy", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestionAnswers_CreatedBy_FormAnswerId_CreatedDate",
                table: "FormQuestionAnswers",
                columns: new[] { "CreatedBy", "FormAnswerId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_AssignmentId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "AssignmentId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_ClassRunId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "ClassRunId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_CreatedBy_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_FormId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "FormId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_OwnerId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "OwnerId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_ResourceId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "ResourceId", "IsDeleted", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormSections_CreatedBy_IsDeleted_CreatedDate",
                table: "FormSections");

            migrationBuilder.DropIndex(
                name: "IX_FormSections_FormId_IsDeleted_CreatedDate",
                table: "FormSections");

            migrationBuilder.DropIndex(
                name: "IX_Forms_CreatedBy_IsDeleted_CreatedDate",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestions_CreatedBy_IsDeleted_CreatedDate",
                table: "FormQuestions");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestions_FormId_IsDeleted_CreatedDate",
                table: "FormQuestions");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestionAnswers_CreatedBy_CreatedDate",
                table: "FormQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestionAnswers_FormAnswerId_CreatedDate",
                table: "FormQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestionAnswers_ScoredBy_CreatedDate",
                table: "FormQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormQuestionAnswers_CreatedBy_FormAnswerId_CreatedDate",
                table: "FormQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_AssignmentId_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_ClassRunId_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_CreatedBy_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_FormId_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_OwnerId_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_ResourceId_IsDeleted_CreatedDate",
                table: "FormAnswers");
        }
    }
}
