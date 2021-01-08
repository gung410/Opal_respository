using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateColAndIndexForPerformanceR2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullTextSearch",
                table: "Course",
                maxLength: 2100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullTextSearchKey",
                table: "Course",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.Sql("UPDATE dbo.Course SET FullTextSearch = COALESCE(CourseName, '') + ' ' + COALESCE(CourseCode, '') + ' ' + COALESCE(ExternalCode, '')");
            migrationBuilder.Sql("UPDATE dbo.Course SET FullTextSearchKey = CONVERT(varchar(60), CreatedDate) + '_' + CONVERT(varchar(40), Id)");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_FullTextSearchKey", "Course");
            migrationBuilder.Sql("CREATE UNIQUE NONCLUSTERED INDEX [IX_Course_FullTextSearchKey] ON [dbo].[Course] ([FullTextSearchKey] DESC)");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_IsDeleted_FullTextSearchKey", "Course");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_IsDeleted_FullTextSearchKey] ON [dbo].[Course] ([IsDeleted] ASC,[FullTextSearchKey] DESC)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_FullTextSearchKey", "Course");

            migrationBuilder.DropColumn(
                name: "FullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "FullTextSearchKey",
                table: "Course");
        }
    }
}
