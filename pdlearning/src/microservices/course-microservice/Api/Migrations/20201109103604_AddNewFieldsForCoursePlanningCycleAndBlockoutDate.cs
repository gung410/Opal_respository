using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewFieldsForCoursePlanningCycleAndBlockoutDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmedBlockoutDate",
                table: "CoursePlanningCycle",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "BlockoutDate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BlockoutDate",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlanningCycle_IsConfirmedBlockoutDate_IsDeleted_CreatedDate",
                table: "CoursePlanningCycle",
                columns: new[] { "IsConfirmedBlockoutDate", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_IsConfirmed_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "IsConfirmed", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_Status_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "Status", "IsDeleted", "FullTextSearchKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoursePlanningCycle_IsConfirmedBlockoutDate_IsDeleted_CreatedDate",
                table: "CoursePlanningCycle");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_IsConfirmed_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_Status_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "IsConfirmedBlockoutDate",
                table: "CoursePlanningCycle");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BlockoutDate");
        }
    }
}
