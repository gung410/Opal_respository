using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateOldLectureDigitalContentConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update LectureContent
                                   set DigitalContentConfig_CanDownload = 0
                                   where Type = 'DigitalContent' and DigitalContentConfig_CanDownload is NULL");
        }
    }
}
