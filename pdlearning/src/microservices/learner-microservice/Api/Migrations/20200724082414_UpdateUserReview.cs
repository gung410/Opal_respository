using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class UpdateUserReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "UserReviews",
                nullable: true);

            migrationBuilder.Sql("UPDATE USERREVIEWS SET CHANGEDDATE = CREATEDDATE WHERE CHANGEDDATE IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "UserReviews");
        }
    }
}
