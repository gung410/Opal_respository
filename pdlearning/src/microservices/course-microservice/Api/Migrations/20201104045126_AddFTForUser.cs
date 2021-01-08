using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddFTForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Users");
            MigrationHelper.DropFullTextCatalogIfExists(migrationBuilder, "FTS_Users");
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_Users]", true);
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_User_FullTextSearchKey", "Users");
            migrationBuilder.Sql("CREATE UNIQUE NONCLUSTERED INDEX [IX_User_FullTextSearchKey] ON [dbo].[Users] ([FullTextSearchKey] DESC)", true);

            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Users(FullTextSearch, ServiceSchemeFullTextSearch,TeachingCourseOfStudyFullTextSearch,DesignationFullTextSearch,DevelopmentalRoleFullTextSearch,TeachingLevelFullTextSearch,TeachingSubjectFullTextSearch,LearningFrameworkFullTextSearch,SystemRolesFullTextSearch) KEY INDEX IX_User_FullTextSearchKey ON FTS_Users WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
