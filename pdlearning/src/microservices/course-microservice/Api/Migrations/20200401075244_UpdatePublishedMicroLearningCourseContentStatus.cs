using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdatePublishedMicroLearningCourseContentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE Course SET ContentStatus = '{ContentStatus.Published}' WHERE PDActivityType = '{MetadataTagConstants.MicroLearningTagId}' AND Status = '{CourseStatus.Published}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // None
        }
    }
}
