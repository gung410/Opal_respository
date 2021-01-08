using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class ImproveSpt22_UseFullTextSearchForMultiValuesColumn_UpdateIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE dbo.Course SET CollaborativeContentCreatorIdsFullTextSearch = CollaborativeContentCreatorIds, CourseCoFacilitatorIdsFullTextSearch = CourseCoFacilitatorIds, CourseFacilitatorIdsFullTextSearch = CourseFacilitatorIds");
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(FullTextSearch,CollaborativeContentCreatorIdsFullTextSearch,CourseCoFacilitatorIdsFullTextSearch,CourseFacilitatorIdsFullTextSearch) KEY INDEX IX_Course_FullTextSearchKey ON FTS_Courses WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(FullTextSearch) KEY INDEX IX_Course_FullTextSearchKey ON FTS_Courses WITH (STOPLIST=OFF)", true);
        }
    }
}
