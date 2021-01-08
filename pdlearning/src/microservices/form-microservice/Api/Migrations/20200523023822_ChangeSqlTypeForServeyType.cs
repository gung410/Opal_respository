using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class ChangeSqlTypeForServeyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SurveyType",
                table: "Forms",
                type: "varchar(30)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = 'PreCourse' WHERE [Type] = 'Survey' and SurveyType = '0'");
            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = 'PostCourse' WHERE [Type] = 'Survey' and SurveyType = '1'");
            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = 'FollowUpPostCourse' WHERE [Type] = 'Survey' and SurveyType = '2'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = '0' WHERE [Type] = 'Survey' and SurveyType = 'PreCourse'");
            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = '1' WHERE [Type] = 'Survey' and SurveyType = 'PostCourse'");
            migrationBuilder.Sql("UPDATE [dbo].[Forms] SET SurveyType = '2' WHERE [Type] = 'Survey' and SurveyType = 'FollowUpPostCourse'");

            migrationBuilder.AlterColumn<int>(
                name: "SurveyType",
                table: "Forms",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldNullable: true);
        }
    }
}
