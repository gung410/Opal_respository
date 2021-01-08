using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddFormAndFormParticipantTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType",
                table: "MyOutstandingTasks");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "MyOutstandingTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FormParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormOriginalObjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormParticipants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    SurveyType = table.Column<string>(type: "varchar(30)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalObjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StandaloneMode = table.Column<string>(type: "varchar(30)", nullable: true),
                    IsStandalone = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyOutstandingTasks_ItemType_ItemId",
                table: "MyOutstandingTasks",
                columns: new[] { "ItemType", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_MyOutstandingTasks_UserId_Priority",
                table: "MyOutstandingTasks",
                columns: new[] { "UserId", "Priority" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormParticipants");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_MyOutstandingTasks_ItemType_ItemId",
                table: "MyOutstandingTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyOutstandingTasks_UserId_Priority",
                table: "MyOutstandingTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MyOutstandingTasks");

            migrationBuilder.CreateIndex(
                name: "IX_MyOutstandingTasks_UserId_ItemType",
                table: "MyOutstandingTasks",
                columns: new[] { "UserId", "ItemType" });
        }
    }
}
