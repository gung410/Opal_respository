using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateDefaultEcertificateTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                 UPDATE [dbo].[ECertificateTemplate]
                  SET ECertificateLayoutId = '0FE84AC9-CF78-4173-8AAB-99DC9CD206D9'
                  WHERE Title = 'E-certificate template 1'

                  UPDATE [dbo].[ECertificateTemplate]
                  SET ECertificateLayoutId = 'A169F864-0190-49CE-B427-501600D246F9'
                  WHERE Title = 'E-certificate template 2'

                  UPDATE [dbo].[ECertificateTemplate]
                  SET ECertificateLayoutId = '4829E987-D26E-4580-AD5E-5C1B0865A9E8'
                  WHERE Title = 'E-certificate template 3'

                  UPDATE [dbo].[ECertificateTemplate]
                  SET ECertificateLayoutId = '960D8DBC-5FB8-48B5-B528-3DF57CA21A23'
                  WHERE Title = 'E-certificate template 4'

                  UPDATE [dbo].[ECertificateTemplate]
                  SET IsSystem = 1, Params = '[]', FullTextSearch = Title + ' (Default)', Title = Title + ' (Default)', Status = 'Active'
                  WHERE Title IN ('E-certificate template 1', 'E-certificate template 2', 'E-certificate template 3', 'E-certificate template 4')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
