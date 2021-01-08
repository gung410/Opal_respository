using Microservice.Course.Common.Helpers;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFTSMaxLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_BlockoutDate_StartDay_StartMonth_EndDay_EndMonth", "BlockoutDate");

            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Users");
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2100)",
                oldMaxLength: 2100,
                oldNullable: true);
            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.Users(FullTextSearch) KEY INDEX IX_Users_FullTextSearchKey ON FTSUsers WITH (STOPLIST=OFF);", true);

            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "Course",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2100)",
                oldMaxLength: 2100,
                oldNullable: true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(FullTextSearch,CollaborativeContentCreatorIdsFullTextSearch,CourseCoFacilitatorIdsFullTextSearch,CourseFacilitatorIdsFullTextSearch) KEY INDEX IX_Course_FullTextSearchKey ON FTS_Courses WITH (STOPLIST=OFF)", true);

            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "AnnouncementTemplate");
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "AnnouncementTemplate",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2100)",
                oldMaxLength: 2100,
                oldNullable: true);
            MigrationHelper.DropFullTextCatalogIfExists(migrationBuilder, "FTSAnnouncementTemplates");
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTSAnnouncementTemplates]", true);
            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.AnnouncementTemplate(FullTextSearch) KEY INDEX IX_AnnouncementTemplate_FullTextSearchKey ON FTSAnnouncementTemplates WITH (STOPLIST=OFF);", true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "StartMonth", "StartDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidToYear", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "StartMonth", "StartDay", "ValidToYear", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "Users",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "Course",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceSchemesFullTextSearch",
                table: "BlockoutDate",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearch",
                table: "AnnouncementTemplate",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_StartDay_StartMonth_EndDay_EndMonth",
                table: "BlockoutDate",
                columns: new[] { "StartDay", "StartMonth", "EndDay", "EndMonth" });
        }
    }
}
