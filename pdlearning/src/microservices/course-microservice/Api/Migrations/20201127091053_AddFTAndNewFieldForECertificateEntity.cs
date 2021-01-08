using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddFTAndNewFieldForECertificateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "ECertificateTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_IsSystem_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate",
                columns: new[] { "IsSystem", "IsDeleted", "FullTextSearchKey" });

            MigrationHelper.DropFullTextCatalogIfExists(migrationBuilder, "FTS_ECertificateTemplate");
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_ECertificateTemplate]", true);
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ECertificateTemplate_FullTextSearchKey", "ECertificateTemplate");
            migrationBuilder.Sql("CREATE UNIQUE NONCLUSTERED INDEX [IX_ECertificateTemplate_FullTextSearchKey] ON [dbo].[ECertificateTemplate] ([FullTextSearchKey] DESC)", true);

            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.ECertificateTemplate(FullTextSearch) KEY INDEX IX_ECertificateTemplate_FullTextSearchKey ON FTS_ECertificateTemplate WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_IsSystem_IsDeleted_FullTextSearchKey",
                table: "ECertificateTemplate");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "ECertificateTemplate");
        }
    }
}
