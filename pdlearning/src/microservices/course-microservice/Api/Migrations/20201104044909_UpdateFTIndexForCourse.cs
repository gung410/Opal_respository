using Microservice.Course.Common.Helpers;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFTIndexForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(FullTextSearch,CollaborativeContentCreatorIdsFullTextSearch,CourseCoFacilitatorIdsFullTextSearch,CourseFacilitatorIdsFullTextSearch,DevelopmentalRoleIdsFullTextSearch,ServiceSchemeIdsFullTextSearch,SubjectAreaIdsFullTextSearch,LearningFrameworkIdsFullTextSearch,LearningDimensionIdsFullTextSearch,LearningAreaIdsFullTextSearch,LearningSubAreaIdsFullTextSearch,CategoryIdsFullTextSearch,TeachingLevelsFullTextSearch) KEY INDEX IX_Course_FullTextSearchKey ON FTS_Courses WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
