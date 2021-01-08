using Microservice.Course.Domain.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdatePdActivityData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE Course SET PDActivityType = '{MetadataTagConstants.MicroLearningTagId}' WHERE PDActivityType is NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // None
        }
    }
}
