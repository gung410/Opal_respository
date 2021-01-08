using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class Improve_Spt_20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_AlternativeApprovingOfficerId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "AlternativeApprovingOfficerId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_ContentStatus_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "ContentStatus", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_CourseCode_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "CourseCode", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_CreatedBy_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "CreatedBy", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_DepartmentId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "DepartmentId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_ECertificateTemplateId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "ECertificateTemplateId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_ExternalCode_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "ExternalCode", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_ExternalCode_Id",
                table: "Course",
                columns: new[] { "IsDeleted", "ExternalCode", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_FirstAdministratorId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "FirstAdministratorId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_MOEOfficerId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "MOEOfficerId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_PDAreaThemeId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "PDAreaThemeId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_PrimaryApprovingOfficerId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "PrimaryApprovingOfficerId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_SecondAdministratorId_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "SecondAdministratorId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_Status_CreatedDate",
                table: "Course",
                columns: new[] { "IsDeleted", "Status", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_AlternativeApprovingOfficerId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_ContentStatus_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_CourseCode_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_CreatedBy_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_DepartmentId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_ECertificateTemplateId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_ExternalCode_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_ExternalCode_Id",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_FirstAdministratorId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_MOEOfficerId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_PDAreaThemeId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_PrimaryApprovingOfficerId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_SecondAdministratorId_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted_Status_CreatedDate",
                table: "Course");

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
        }
    }
}
