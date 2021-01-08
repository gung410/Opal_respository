using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFullTextSearchForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adding double space between CourseName and CourseCode because some records SQL cannot create full-text index for it and user cannot search exact course's name
            migrationBuilder.Sql("UPDATE dbo.Course SET FullTextSearch = COALESCE(CourseName, '') + '  ' + COALESCE(CourseCode, '') + ' ' + COALESCE(ExternalCode, '')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
