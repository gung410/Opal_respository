using Microservice.Course.Domain.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateLearningModeForCourseMicroLearning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"UPDATE [dbo].[Course]
                SET LearningMode = '{MetadataTagConstants.ELearningTagId}'
                WHERE PDActivityType = '{MetadataTagConstants.MicroLearningTagId}' AND LearningMode IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
