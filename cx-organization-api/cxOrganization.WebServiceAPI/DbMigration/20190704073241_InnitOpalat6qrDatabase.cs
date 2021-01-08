//  This has been run on production, commented out for safety
/*
using System.Configuration;
using System.IO;
using cxOrganization.Domain.Settings;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public partial class InnitOpalat6qrDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (AppSettings.ProjectName.ToLower() == "opal")
            {
                string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "cxOrganization.WebServiceAPI", "DbMigration", "Scripts", "competence_moe_blank_db_20190704.sql");
                migrationBuilder.Sql(File.ReadAllText(sql));
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
*/
