using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddNewFieldsForBlockoutDateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "BlockoutDate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "BlockoutDate",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_EndDate_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "EndDate", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_StartDate_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "StartDate", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.Sql(@"UPDATE BlockoutDate
                                    SET StartDate = DATETIMEFROMPARTS(ValidYear, StartMonth, StartDay, 0, 0, 0, 1),
                                        EndDate = DATETIMEFROMPARTS(ValidYear, EndMonth, EndDay, 23, 59, 59, 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_EndDate_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropIndex(
                name: "IX_BlockoutDate_StartDate_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "BlockoutDate");
        }
    }
}
