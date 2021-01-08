using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Calendar.Migrations
{
    public partial class UseEventBaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPersonalEvents_PersonalEvents_EventId",
                table: "UserPersonalEvents");

            migrationBuilder.DropTable(
                name: "CommunityEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalEvents",
                table: "PersonalEvents");

            migrationBuilder.RenameTable(
                name: "PersonalEvents",
                newName: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Events",
                unicode: false,
                maxLength: 19,
                nullable: false,
                defaultValue: "BaseEvent");

            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "Events",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CommunityId",
                table: "Events",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Communities_CommunityId",
                table: "Events",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPersonalEvents_Events_EventId",
                table: "UserPersonalEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Communities_CommunityId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPersonalEvents_Events_EventId",
                table: "UserPersonalEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CommunityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "PersonalEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalEvents",
                table: "PersonalEvents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CommunityEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityEvents_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityEvents_CommunityId",
                table: "CommunityEvents",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPersonalEvents_PersonalEvents_EventId",
                table: "UserPersonalEvents",
                column: "EventId",
                principalTable: "PersonalEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
