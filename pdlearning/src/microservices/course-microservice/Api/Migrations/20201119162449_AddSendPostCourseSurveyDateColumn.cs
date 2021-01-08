using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddSendPostCourseSurveyDateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SendPostCourseSurveyDate",
                table: "Registration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Registration
                SET SendPostCourseSurveyDate = GetDate()
                where PostCourseEvaluationFormCompleted=0 
                and IsExpired=0 
                and ([Status] ='OfferConfirmed' or [Status] ='ConfirmedByCA')
                and (LearningStatus='Passed' or LearningStatus='Failed')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendPostCourseSurveyDate",
                table: "Registration");
        }
    }
}
