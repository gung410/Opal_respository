using Microservice.Course.Common.Helpers;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class BlockoutDateModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BlockoutDate",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullTextSearch",
                table: "BlockoutDate",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BlockoutDate",
                nullable: true);

            MigrationHelper.DropFullTextIndexIfExists(migrationBuilder, "BlockoutDate");
            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.BlockoutDate(FullTextSearch,ServiceSchemesFullTextSearch) KEY INDEX IX_BlockoutDate_FullTextSearchKey ON FTSBlockoutDate WITH (STOPLIST=OFF);", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "FullTextSearch",
                table: "BlockoutDate");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BlockoutDate");
        }
    }
}
