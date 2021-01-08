using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class MoveReasonClassRunToCommentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM Comment
                                                   WHERE Action LIKE 'classrun-cancellation-pending-approval%' 
			                                          OR Action LIKE 'classrun-reschedule-pending-approval%')
			                        BEGIN				
                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ChangedDate IS NOT NULL then ChangedDate else CURRENT_TIMESTAMP end, 
                                        ChangedBy, 
                                        Id, 
                                        Reason, 
                                        'classrun-cancellation-pending-approval'
                                        FROM ClassRun
                                        WHERE Reason IS NOT NULL
                                        AND RescheduleStatus IS NULL AND CancellationStatus IS NOT NULL

				                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ChangedDate IS NOT NULL then ChangedDate else CURRENT_TIMESTAMP end, 
                                        ChangedBy, 
                                        Id, 
                                        Reason, 
                                        'classrun-reschedule-pending-approval'
                                        FROM ClassRun
                                        WHERE Reason IS NOT NULL
                                        AND RescheduleStatus IS NOT NULL AND CancellationStatus IS NULL

				                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ChangedDate IS NOT NULL then ChangedDate else CURRENT_TIMESTAMP end, 
                                        ChangedBy, 
                                        Id, 
                                        Reason, 
                                        'classrun-cancellation-pending-approval'
                                        FROM ClassRun
                                        WHERE Reason IS NOT NULL
                                        AND RescheduleStatus IS NOT NULL AND CancellationStatus IS NOT NULL
			                        END");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ClassRun");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
