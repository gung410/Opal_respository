using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateColAndIndexForPerformanceR2_UpdateFullTextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(FullTextSearch) KEY INDEX IX_Course_FullTextSearchKey ON FTS_Courses WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(CourseName,CourseCode,ExternalCode) KEY INDEX PK_Course ON FTS_Courses WITH (STOPLIST=OFF)", true);
        }
    }
}
