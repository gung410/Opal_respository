using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewFieldsForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Course] SET CourseType = 'New'");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Course",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<int>(
                name: "DurationMinutes",
                table: "Course",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CourseType",
                table: "Course",
                unicode: false,
                maxLength: 15,
                nullable: false,
                defaultValue: "New",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Course",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "CourseLevel",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseCode",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNonCommerInMOEReuseWithModification",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNonCommerInMoeReuseWithoutModification",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNonCommerReuseWithModification",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowNonCommerReuseWithoutModification",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPersonalDownload",
                table: "Course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeApprovingOfficerId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableBranchIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableClusterIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableDivisionIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableSchoolIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableZoneIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CocurricularActivityIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollaborativeContentCreatorIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CopyrightOwner",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseFacilitatorIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CourseFee",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseOutlineStructure",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevelopmentalRoleIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ECertificatePrerequisite",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ECertificateTemplateId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EasSubstantiveGradeBandingIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FirstAdministratorId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobFamily",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningAreaIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningDimensionIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningFrameworkIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningMode",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningSubAreaIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MOEOfficerEmail",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MOEOfficerId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MOEOfficerPhoneNumber",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipantPerClass",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumPlacesPerSchool",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinParticipantPerClass",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NatureOfCourse",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NotionalCost",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfBeginningTeacher",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfHoursPerClass",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfHoursPerSession",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfMiddleManagement",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfPlannedClass",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfSchoolLeader",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfSeniorOrLeadTeacher",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumOfSessionPerClass",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTrainingAgencyReason",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerBranchIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerDivisionIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PDActivityType",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PDAreaThemeCode",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PDAreaThemeId",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerOrganisationIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdActivityPeriods",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfWork",
                table: "Course",
                maxLength: 50,
                nullable: false,
                defaultValue: "ApplicableForEveryone");

            migrationBuilder.AddColumn<Guid>(
                name: "PostCourseEvaluationFormId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreCourseEvaluationFormId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrerequisiteCourseIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryApprovingOfficerId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationMethod",
                table: "Course",
                maxLength: 50,
                nullable: false,
                defaultValue: "RegistrationOrNomination");

            migrationBuilder.AddColumn<Guid>(
                name: "SecondAdministratorId",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSchemeIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectAreaIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherOutcomeIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingCourseStudyIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingLevels",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeachingSubjectIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalHoursAttendWithinYear",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackIds",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingAgency",
                table: "Course",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraisiCourseCode",
                table: "Course",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowNonCommerInMOEReuseWithModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AllowNonCommerInMoeReuseWithoutModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AllowNonCommerReuseWithModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AllowNonCommerReuseWithoutModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AllowPersonalDownload",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AlternativeApprovingOfficerId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApplicableBranchIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApplicableClusterIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApplicableDivisionIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApplicableSchoolIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApplicableZoneIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CategoryIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CocurricularActivityIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CollaborativeContentCreatorIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CopyrightOwner",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseFacilitatorIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseFee",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseOutlineStructure",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "DevelopmentalRoleIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ECertificatePrerequisite",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ECertificateTemplateId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "EasSubstantiveGradeBandingIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "FirstAdministratorId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "JobFamily",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningAreaIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningDimensionIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningFrameworkIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningMode",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LearningSubAreaIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MOEOfficerEmail",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MOEOfficerId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MOEOfficerPhoneNumber",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MaxParticipantPerClass",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MaximumPlacesPerSchool",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MinParticipantPerClass",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NatureOfCourse",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NotionalCost",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfBeginningTeacher",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfHoursPerClass",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfHoursPerSession",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfMiddleManagement",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfPlannedClass",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfSchoolLeader",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfSeniorOrLeadTeacher",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "NumOfSessionPerClass",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "OtherTrainingAgencyReason",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "OwnerBranchIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "OwnerDivisionIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PDActivityType",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PDAreaThemeCode",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PDAreaThemeId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PartnerOrganisationIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PdActivityPeriods",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PlaceOfWork",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PostCourseEvaluationFormId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PreCourseEvaluationFormId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PrerequisiteCourseIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "PrimaryApprovingOfficerId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "RegistrationMethod",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SecondAdministratorId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ServiceSchemeIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SubjectAreaIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeacherOutcomeIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeachingCourseStudyIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeachingLevels",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeachingSubjectIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TotalHoursAttendWithinYear",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TrackIds",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TrainingAgency",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TraisiCourseCode",
                table: "Course");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Course",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 30,
                oldDefaultValue: "Draft");

            migrationBuilder.AlterColumn<int>(
                name: "DurationMinutes",
                table: "Course",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseType",
                table: "Course",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 15,
                oldDefaultValue: "New");

            migrationBuilder.AlterColumn<string>(
                name: "CourseName",
                table: "Course",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseLevel",
                table: "Course",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseCode",
                table: "Course",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
