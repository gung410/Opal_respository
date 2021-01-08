using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class SetRegistrationToBeExpiredForCancelledClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                Update Registration set IsExpired=1
                from Registration r
                join ClassRun cr on cr.Id=r.ClassRunId
                where cr.Status='Cancelled'
                and r.Status NOT IN ('Rejected','RejectedByCA','AddedByCAConflict','AddedByCAClassfull')
                and r.LearningStatus <> 'Completed'
                and r.IsExpired=0
                and (r.WithdrawalStatus is null or r.WithdrawalStatus<>'Withdrawn')
                and (r.ClassRunChangeStatus is null or r.ClassRunChangeStatus<>'ConfirmedByCA')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
