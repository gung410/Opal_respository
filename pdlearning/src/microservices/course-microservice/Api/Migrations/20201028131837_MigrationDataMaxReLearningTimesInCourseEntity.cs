using Microservice.Course.Domain.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class MigrationDataMaxReLearningTimesInCourseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"UPDATE Course
                                   SET MaxReLearningTimes = {int.MaxValue}
                                   WHERE PDActivityType = '{MetadataTagConstants.MicroLearningTagId}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
