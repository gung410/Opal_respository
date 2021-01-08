using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class CreateTrackingActivityStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROC TrackingLearningActivity
                             @itemId         UNIQUEIDENTIFIER, 
                             @trackingType   VARCHAR(50), 
                             @trackingAction VARCHAR(50)
                             AS
                                 IF EXISTS
                                 (
                                     SELECT 1
                                     FROM LearningTrackings
                                     WHERE ItemId = @itemId
                                           AND TrackingType = @trackingType
                                           AND TrackingAction = @trackingAction
                                 )
                                     UPDATE LearningTrackings
                                       SET 
                                           TotalCount+=1
                                     WHERE ItemId = @itemId
                                           AND TrackingType = @trackingType
                                           AND TrackingAction = @trackingAction;
                                     ELSE
                                     INSERT INTO LearningTrackings
                                     (Id, CreatedDate, ChangedDate, ItemId, TrackingType, TrackingAction, TotalCount)
                                     VALUES (NEWID(), GETDATE(), GETDATE(), @itemId, @trackingType, @trackingAction, 1);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROC TrackingLearningActivity");
        }
    }
}
