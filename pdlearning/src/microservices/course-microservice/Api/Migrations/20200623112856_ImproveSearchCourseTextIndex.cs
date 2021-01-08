using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class ImproveSearchCourseTextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(CourseName,CourseCode,ExternalCode) KEY INDEX PK_Course ON FTS_Courses", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(CourseName,CourseCode) KEY INDEX PK_Course ON FTS_Courses", true);
        }
    }
}
