using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public partial class CreatePrcUserListGet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                        "cxOrganization.WebServiceAPI", "DbMigration", "Scripts", "20200508180647_CreatePrcUserListGet.sql");
            migrationBuilder.Sql(File.ReadAllText(sql));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE dbo.prc_UserList_get");

        }
    }
}
