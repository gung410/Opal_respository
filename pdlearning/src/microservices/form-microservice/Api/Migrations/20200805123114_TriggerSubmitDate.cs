using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class TriggerSubmitDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Forms set SubmitDate = ChangedDate  where SubmitDate is Null and Status = 'PendingApproval'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
