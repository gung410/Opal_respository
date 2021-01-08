using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.StandaloneSurvey.Migrations
{
    public partial class AddColsRelatedCls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "Forms",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Forms");
        }
    }
}
