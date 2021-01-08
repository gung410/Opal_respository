using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class RemoveCommunityRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communities_Communities_ParentId",
                table: "Communities");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityMemberships_Communities_CommunityId",
                table: "CommunityMemberships");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_Communities_ParentId",
                table: "Communities",
                column: "ParentId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communities_Communities_ParentId",
                table: "Communities");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_Communities_ParentId",
                table: "Communities",
                column: "ParentId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityMemberships_Communities_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
