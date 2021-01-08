using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class ImproveSpt2_2ForCourse_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ContentStatus_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CourseCode_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedBy_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_DepartmentId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ECertificateTemplateId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ExternalCode_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_FirstAdministratorId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_MOEOfficerId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PDAreaThemeId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_SecondAdministratorId_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedBy_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_FirstAdministratorId_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_SecondAdministratorId_Status_IsDeleted_CreatedDate",
                table: "Course");

            migrationBuilder.CreateIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "AlternativeApprovingOfficerId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ContentStatus_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "ContentStatus", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CourseCode_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "CourseCode", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "CreatedBy", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_DepartmentId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "DepartmentId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ECertificateTemplateId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "ECertificateTemplateId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ExternalCode_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "ExternalCode", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_FirstAdministratorId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "FirstAdministratorId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_MOEOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "MOEOfficerId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PDAreaThemeId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "PDAreaThemeId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "PrimaryApprovingOfficerId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_SecondAdministratorId_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "SecondAdministratorId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "Status", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "AlternativeApprovingOfficerId", "Status", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "CreatedBy", "Status", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_FirstAdministratorId_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "FirstAdministratorId", "Status", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "PrimaryApprovingOfficerId", "Status", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_SecondAdministratorId_Status_IsDeleted_FullTextSearchKey",
                table: "Course",
                columns: new[] { "SecondAdministratorId", "Status", "IsDeleted", "FullTextSearchKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ContentStatus_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CourseCode_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedBy_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_DepartmentId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ECertificateTemplateId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_ExternalCode_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_FirstAdministratorId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_MOEOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PDAreaThemeId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_SecondAdministratorId_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedBy_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_FirstAdministratorId_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_SecondAdministratorId_Status_IsDeleted_FullTextSearchKey",
                table: "Course");

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "AlternativeApprovingOfficerId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ContentStatus_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "ContentStatus", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CourseCode_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "CourseCode", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_DepartmentId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "DepartmentId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ECertificateTemplateId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "ECertificateTemplateId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ExternalCode_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "ExternalCode", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_FirstAdministratorId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "FirstAdministratorId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_MOEOfficerId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "MOEOfficerId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PDAreaThemeId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "PDAreaThemeId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "PrimaryApprovingOfficerId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_SecondAdministratorId_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "SecondAdministratorId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_AlternativeApprovingOfficerId_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "AlternativeApprovingOfficerId", "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "CreatedBy", "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_FirstAdministratorId_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "FirstAdministratorId", "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_PrimaryApprovingOfficerId_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "PrimaryApprovingOfficerId", "Status", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_SecondAdministratorId_Status_IsDeleted_CreatedDate",
                table: "Course",
                columns: new[] { "SecondAdministratorId", "Status", "IsDeleted", "CreatedDate" });
        }
    }
}
