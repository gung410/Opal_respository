using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddScriptMigrateDataAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE QuizAssignmentFormQuestion
                                   SET Question_Type = 'FreeText'
                                   WHERE Question_Type = 'ShortText' OR Question_Type = 'LongText'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
