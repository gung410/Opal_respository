using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateCourseAutomateActivatedForClassrun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTE: Update CourseAutomateActivated of Classrun which have Learning Mode is E-Learning and Registration Method is Public.
            migrationBuilder.Sql($@"UPDATE [dbo].[ClassRun]
                                    SET CourseAutomateActivated = 1
                                    FROM [dbo].[ClassRun] as classrun 
                                    JOIN [dbo].[Course] c on classrun.CourseId = c.Id
                                    where c.LearningMode = '{MetadataTagConstants.ELearningTagId}' AND 
	                                    c.RegistrationMethod = '{RegistrationMethod.Public.ToString()}' AND c.PDActivityType != '{MetadataTagConstants.MicroLearningTagId}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
