using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class BlockoutDateLogicUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidFromYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidToYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "ValidFromYear",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "ValidToYear",
                table: "BlockoutDate");

            migrationBuilder.AddColumn<Guid>(
                name: "PlanningCycleId",
                table: "BlockoutDate",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ValidYear",
                table: "BlockoutDate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_PlanningCycleId",
                table: "BlockoutDate",
                column: "PlanningCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidYear", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidYear", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidYear", "StartMonth", "StartDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidYear_StartMonth_StartDay_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidYear", "StartMonth", "StartDay", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlockoutDate_CoursePlanningCycle_PlanningCycleId",
                table: "BlockoutDate",
                column: "PlanningCycleId",
                principalTable: "CoursePlanningCycle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockoutDate_CoursePlanningCycle_PlanningCycleId",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_PlanningCycleId",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_ValidYear_StartMonth_StartDay_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "PlanningCycleId",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "ValidYear",
                table: "BlockoutDate");

            migrationBuilder.AddColumn<int>(
                name: "ValidFromYear",
                table: "BlockoutDate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ValidToYear",
                table: "BlockoutDate",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidToYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidToYear", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "StartMonth", "StartDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidToYear", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_StartMonth_StartDay_ValidToYear_EndMonth_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "StartMonth", "StartDay", "ValidToYear", "EndMonth", "EndDay", "IsDeleted", "FullTextSearchKey" });
        }
    }
}
