using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateSearchTextIndexing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_Courses]", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Course(CourseName,CourseCode) KEY INDEX PK_Course ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.ClassRun(ClassTitle,ClassRunCode) KEY INDEX PK_ClassRun ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Session(SessionTitle) KEY INDEX PK_Session ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Lecture(LectureName,Description) KEY INDEX PK_Lecture ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Section(Title,Description) KEY INDEX PK_Section ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Assignment(Title) KEY INDEX PK_Assignment ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.LearningPath(Title) KEY INDEX PK_LearningPath ON FTS_Courses", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.HierarchyDepartments(Path) KEY INDEX PK_HierarchyDepartments ON FTS_Courses", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "Course");
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.ClassRun", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.Session", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.Lecture", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.Section", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.Assignment", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.LearningPath", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.HierarchyDepartments", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG FTS_Courses", true);
        }
    }
}
