using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UseFullTextSearchForFilterColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CocurricularActivity",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignationFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevelopmentalRole",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevelopmentalRoleFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EasSubstantiveGradeBanding",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobFamily",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningFramework",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningFrameworkFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceScheme",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSchemeFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemRoles",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemRolesFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingCourseOfStudy",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingCourseOfStudyFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingLevel",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingLevelFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingSubject",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingSubjectFullTextSearch",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Track",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevelopmentalRoleIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningAreaIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningDimensionIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningFrameworkIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningSubAreaIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSchemeIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectAreaIdsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingLevelsFullTextSearch",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoFacilitatorIdsFullTextSearch",
                table: "ClassRun",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilitatorIdsFullTextSearch",
                table: "ClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CocurricularActivity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DesignationFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DevelopmentalRole",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DevelopmentalRoleFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EasSubstantiveGradeBanding",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobFamily",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LearningFramework",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LearningFrameworkFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServiceScheme",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServiceSchemeFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SystemRoles",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SystemRolesFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingCourseOfStudy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingCourseOfStudyFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingLevelFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingSubject",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TeachingSubjectFullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Track",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CategoryIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "DevelopmentalRoleIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningAreaIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningDimensionIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningFrameworkIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningSubAreaIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ServiceSchemeIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SubjectAreaIdsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeachingLevelsFullTextSearch",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CoFacilitatorIdsFullTextSearch",
                table: "ClassRun");

            migrationBuilder.DropColumn(
                name: "FacilitatorIdsFullTextSearch",
                table: "ClassRun");
        }
    }
}
