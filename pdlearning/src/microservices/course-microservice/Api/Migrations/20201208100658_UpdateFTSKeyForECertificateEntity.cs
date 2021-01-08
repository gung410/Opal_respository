using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFTSKeyForECertificateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "ECertificateTemplate");
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearchKey",
                table: "ECertificateTemplate",
                type: "nvarchar(200)",
                unicode: false,
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldDefaultValue: string.Empty);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.ECertificateTemplate(FullTextSearch) KEY INDEX IX_ECertificateTemplate_FullTextSearchKey ON FTS_ECertificateTemplate WITH (STOPLIST=OFF)", true);

            migrationBuilder.Sql(
                @"UPDATE dbo.ECertificateTemplate
                SET FullTextSearchKey = CONVERT(nvarchar(100), REPLACE(SUBSTRING(COALESCE(Title, ''), 0, 100), ' ', '')) + '_' + CONVERT(varchar(40), Id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearchKey",
                table: "ECertificateTemplate",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldUnicode: false,
                oldDefaultValue: string.Empty);
        }
    }
}
