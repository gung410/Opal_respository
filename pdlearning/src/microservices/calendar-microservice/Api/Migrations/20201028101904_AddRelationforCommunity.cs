using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class AddRelationforCommunity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEvents",
                table: "UserEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "UserEvents",
                newName: "UserPersonalEvents");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "PersonalEvents");

            migrationBuilder.RenameIndex(
                name: "IX_UserEvents_EventId_UserId",
                table: "UserPersonalEvents",
                newName: "IX_UserPersonalEvents_EventId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserEvents_UserId",
                table: "UserPersonalEvents",
                newName: "IX_UserPersonalEvents_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPersonalEvents",
                table: "UserPersonalEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalEvents",
                table: "PersonalEvents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityMemberships_Communities_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPersonalEvents_PersonalEvents_EventId",
                table: "UserPersonalEvents",
                column: "EventId",
                principalTable: "PersonalEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityMemberships_Communities_CommunityId",
                table: "CommunityMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPersonalEvents_PersonalEvents_EventId",
                table: "UserPersonalEvents");

            migrationBuilder.DropIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPersonalEvents",
                table: "UserPersonalEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalEvents",
                table: "PersonalEvents");

            migrationBuilder.RenameTable(
                name: "UserPersonalEvents",
                newName: "UserEvents");

            migrationBuilder.RenameTable(
                name: "PersonalEvents",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_UserPersonalEvents_EventId_UserId",
                table: "UserEvents",
                newName: "IX_UserEvents_EventId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPersonalEvents_UserId",
                table: "UserEvents",
                newName: "IX_UserEvents_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEvents",
                table: "UserEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_Events_EventId",
                table: "UserEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
