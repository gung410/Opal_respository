using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFTSKeyForBlockoutDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE dbo.BlockoutDate
            SET FullTextSearchKey = CONVERT(varchar(10), ValidYear) + '/' + CONVERT(varchar(10), FORMAT(StartMonth, '00')) + '/' + CONVERT(varchar(10), FORMAT(StartDay, '00')) + '_' + CONVERT(varchar(40), UPPER(Id))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
