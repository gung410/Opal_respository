using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveIndexForCourseNameAndClassRunTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Because RDS Devop problem, maximum indexing is 1700 byte
            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT * FROM sys.indexes
                                    WHERE name = 'IX_Course_CourseName' AND object_id = OBJECT_ID('dbo.Course'))
                                    DROP INDEX IX_Course_CourseName ON Course; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT * FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_ClassTitle' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_ClassTitle ON ClassRun; ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
