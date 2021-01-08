using System;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddBlockoutDateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockoutDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: true),
                    StartDay = table.Column<int>(nullable: false),
                    StartMonth = table.Column<int>(nullable: false),
                    EndDay = table.Column<int>(nullable: false),
                    EndMonth = table.Column<int>(nullable: false),
                    ValidFromYear = table.Column<int>(nullable: false),
                    ValidToYear = table.Column<int>(nullable: true),
                    ServiceSchemes = table.Column<string>(nullable: true),
                    ServiceSchemesFullTextSearch = table.Column<string>(nullable: true),
                    FullTextSearchKey = table.Column<string>(unicode: false, maxLength: 100, nullable: false, defaultValue: string.Empty)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockoutDate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_FullTextSearchKey",
                table: "BlockoutDate",
                column: "FullTextSearchKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_IsDeleted_ChangedDate",
                table: "BlockoutDate",
                columns: new[] { "IsDeleted", "ChangedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_CreatedBy_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "CreatedBy", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_EndDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "EndDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_EndMonth_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "EndMonth", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_StartDay_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "StartDay", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_StartMonth_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "StartMonth", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_UpdatedBy_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "UpdatedBy", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidFromYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidFromYear", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_ValidToYear_IsDeleted_FullTextSearchKey",
                table: "BlockoutDate",
                columns: new[] { "ValidToYear", "IsDeleted", "FullTextSearchKey" });

            migrationBuilder.CreateIndex(
                name: "IX_BlockoutDate_StartDay_StartMonth_EndDay_EndMonth",
                table: "BlockoutDate",
                columns: new[] { "StartDay", "StartMonth", "EndDay", "EndMonth" });

            MigrationHelper.DropFullTextCatalogIfExists(migrationBuilder, "FTSBlockoutDate");
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTSBlockoutDate]", true);

            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.BlockoutDate(ServiceSchemesFullTextSearch) KEY INDEX IX_BlockoutDate_FullTextSearchKey ON FTSBlockoutDate WITH (STOPLIST=OFF);", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockoutDate");
        }
    }
}
