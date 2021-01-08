using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class CreateFulltextContentTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_DigitalContents]WITH ACCENT_SENSITIVITY = ON", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.DigitalContents(Title) KEY INDEX PK_DigitalContents ON FTS_DigitalContents", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.DigitalContents", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG FTS_DigitalContents", true);
        }
    }
}
