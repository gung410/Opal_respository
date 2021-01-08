using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class CreateFullTextIndexForFormsTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_Forms]WITH ACCENT_SENSITIVITY = ON", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.Forms(Title) KEY INDEX PK_Forms ON FTS_Forms", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.Forms", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG FTS_Forms", true);
        }
    }
}
