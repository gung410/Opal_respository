using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class RenameIsUneditable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Question_IsUneditable", "FormQuestions", "Question_IsSurveyTemplateQuestion");
            migrationBuilder.RenameColumn("Question_IsUneditable", "SharedQuestions", "Question_IsSurveyTemplateQuestion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("Question_IsSurveyTemplateQuestion", "FormQuestions", "Question_IsUneditable");
            migrationBuilder.RenameColumn("Question_IsSurveyTemplateQuestion", "SharedQuestions", "Question_IsUneditable");
        }
    }
}
