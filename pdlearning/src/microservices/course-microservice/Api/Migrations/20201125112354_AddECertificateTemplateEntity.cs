using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddECertificateTemplateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhysicalFileName",
                table: "ECertificateTemplate",
                newName: "Params");

            migrationBuilder.AddColumn<Guid>(
                name: "CompleteCourseECertificateId",
                table: "Registration",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ECertificateTemplate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ECertificateTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ECertificateLayoutId",
                table: "ECertificateTemplate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FullTextSearch",
                table: "ECertificateTemplate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullTextSearchKey",
                table: "ECertificateTemplate",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ECertificateTemplate",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ECertificateTemplate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_CreatedBy_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate",
                columns: new[] { "CreatedBy", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_ECertificateLayoutId_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate",
                columns: new[] { "ECertificateLayoutId", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.Sql("UPDATE dbo.ECertificateTemplate SET FullTextSearchKey = CONVERT(varchar(60), CreatedDate) + '_' + CONVERT(varchar(40), Id)");

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_FullTextSearchKey",
                table: "ECertificateTemplate",
                column: "FullTextSearchKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_IsDeleted_ChangedDate",
                table: "ECertificateTemplate",
                columns: new[] { "IsDeleted", "ChangedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate",
                columns: new[] { "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_Status_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate",
                columns: new[] { "Status", "IsDeleted", "FullTextSearchKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_CreatedBy_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_ECertificateLayoutId_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_IsDeleted_ChangedDate",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_Status_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "CompleteCourseECertificateId",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "ECertificateLayoutId",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "FullTextSearch",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ECertificateTemplate");

            migrationBuilder.RenameColumn(
                name: "Params",
                table: "ECertificateTemplate",
                newName: "PhysicalFileName");
        }
    }
}
