using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class CorrectFullTextSearchKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE [dbo].[Course]
                SET FullTextSearchKey = CONVERT(varchar(60), CreatedDate) + '_' + CONVERT(varchar(40),Id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
