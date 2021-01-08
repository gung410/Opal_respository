using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class MigrateIdentifierOfCourseClassRunAndDigitalContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE ClassRun SET Id = ClassRunId;");
            migrationBuilder.Sql("UPDATE DigitalContents SET Id = DigitalContentId;");
            migrationBuilder.Sql("UPDATE Course SET Id = CourseId;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
