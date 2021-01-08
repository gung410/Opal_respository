using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNieAcademicGroupFieldForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NieAcademicGroups",
                table: "Course",
                nullable: true);
            migrationBuilder.Sql("Update Course set NieAcademicGroups='[\"AsianLanguagesAndCultures\",\"EnglishLanguageAndLiterature\",\"LearningSciencesAndAssessment\",\"PolicyCurriculumAndLeadership\",\"PsychologyAndChildHumanDevelopment\",\"VisualAndPerformingArts\",\"HumanitiesAndSocialStudiesEducation\",\"MathematicsAndMathematicsEducation\",\"NaturalSciencesAndScienceEducation\",\"PhysicalEducationAndSportsScience\"]' where TrainingAgency like '%NIE%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NieAcademicGroups",
                table: "Course");
        }
    }
}
