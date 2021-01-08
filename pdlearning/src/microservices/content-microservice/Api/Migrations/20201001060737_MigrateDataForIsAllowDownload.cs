using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class MigrateDataForIsAllowDownload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE DigitalContents SET IsAllowDownload = 'true' WHERE TermsOfUse IS NOT NULL AND TermsOfUse != ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
