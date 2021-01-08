using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public partial class AlterPrcUserListGetFilterStatusTypeLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                "cxOrganization.WebServiceAPI", "DbMigration", "Scripts", "20200527105955_AlterPrcUserListGetFilterStatusTypeLog.sql");
            migrationBuilder.Sql(File.ReadAllText(sql));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                "cxOrganization.WebServiceAPI", "DbMigration", "Scripts", "20200508180647_CreatePrcUserListGet.sql");
            migrationBuilder.Sql(File.ReadAllText(sql));
        }
    }
}
