using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Migrations
{
    public partial class InsertDataToECertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = string.Empty;
            sql += $@"Insert into [dbo].[ECertificateTemplate] (Id, Title, PhysicalFileName, IsDeleted, CreatedDate) values
                    ('{Guid.NewGuid()}', 'E-certificate template 1', 'ecertificate_template_1.trdp', 0, '{Clock.Now}'),
                    ('{Guid.NewGuid()}', 'E-certificate template 2', 'ecertificate_template_2.trdp', 0, '{Clock.Now}'),
                    ('{Guid.NewGuid()}', 'E-certificate template 3', 'ecertificate_template_3.trdp', 0, '{Clock.Now}'),
                    ('{Guid.NewGuid()}', 'E-certificate template 4', 'ecertificate_template_4.trdp', 0, '{Clock.Now}');";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
