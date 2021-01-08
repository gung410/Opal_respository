using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class TriggerSubmitDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update DigitalContents set SubmitDate = ChangedDate  where SubmitDate is Null and Status = 'PendingForApproval'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
