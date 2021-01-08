using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public partial class CreateFuncGetPathDepartmentID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                "cxOrganization.WebServiceAPI", "DbMigration", "Scripts", "20200623034646_CreateFuncGetPathDepartmentID.sql");
            migrationBuilder.Sql(File.ReadAllText(sql));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
