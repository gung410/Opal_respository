using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveCommentInRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (
                                    SELECT 1 FROM Comment
                                    WHERE Action LIKE 'registration%')

                                    BEGIN
                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ApprovingDate IS NOT NULL then ApprovingDate else CURRENT_TIMESTAMP end, 
                                        CommentBy, 
                                        Id, 
                                        Comment, 
                                        Case when [Status] = 'WaitlistConfirmed' then 'registration-waitlist-confirmed' when [Status] = 'Approved' then 'registration-approved' else 'registration-rejected' end
                                        FROM Registration
                                        WHERE Comment IS NOT NULL 
                                        and ClassRunChangeStatus IS NULL 
                                        and WithdrawalStatus IS NULL 
                                        and CommentBy IS NOT NULL
                                        and ([Status]='Approved' or [Status]='Rejected' or [Status]='WaitlistConfirmed')

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when AdministrationDate IS NOT NULL then AdministrationDate else CURRENT_TIMESTAMP end, 
                                        AdministratorCommentBy, 
                                        Id, 
                                        AdministratorComment, 
                                        Case when [Status] = 'ConfirmedByCA' then 'registration-confirmed-by-ca' when [Status] = 'RejectedByCA' then 'registration-rejected-by-ca' else 'registration-waitlist-pending-approval-by-learner' end
                                        FROM Registration
                                        WHERE AdministratorComment IS NOT NULL
                                        and ClassRunChangeStatus IS NULL 
                                        and WithdrawalStatus IS NULL 
                                        and AdministratorCommentBy IS NOT NULL
                                        and ([Status]='ConfirmedByCA' or [Status]='RejectedByCA' or [Status]='WaitlistPendingApprovalByLearner')

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ApprovingDate IS NOT NULL then ApprovingDate else CURRENT_TIMESTAMP end, 
                                        CommentBy, 
                                        Id, 
                                        Comment, 
                                        Case when [Status] = 'Approved' then 'registration-classrun-change-approved' else 'registration-classrun-change-rejected' end
                                        FROM Registration
                                        WHERE Comment IS NOT NULL 
                                        and WithdrawalStatus IS NULL 
                                        and CommentBy IS NOT NULL
                                        and (ClassRunChangeStatus='Approved' or ClassRunChangeStatus='Rejected')

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ApprovingDate IS NOT NULL then ApprovingDate else CURRENT_TIMESTAMP end, 
                                        ReasonBy, 
                                        Id, 
                                        Reason, 
                                        'registration-classrun-change-pending-confirmation'
                                        FROM Registration
                                        WHERE Reason IS NOT NULL 
                                        and WithdrawalStatus IS NULL 
                                        and ReasonBy IS NOT NULL
                                        and ClassRunChangeStatus='PendingConfirmation'

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when AdministrationDate IS NOT NULL then AdministrationDate else CURRENT_TIMESTAMP end,
                                        AdministratorCommentBy, 
                                        Id, 
                                        AdministratorComment, 
                                        Case when [Status] = 'ConfirmedByCA' then 'registration-classrun-change-confirmed-by-ca' else 'registration-classrun-change-rejected-by-ca' end
                                        FROM Registration
                                        WHERE AdministratorComment is not null 
                                        and WithdrawalStatus IS NULL 
                                        and AdministratorCommentBy IS NOT NULL
                                        and (ClassRunChangeStatus='ConfirmedByCA' or ClassRunChangeStatus='RejectedByCA')

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ChangedDate IS NOT NULL then ChangedDate else CURRENT_TIMESTAMP end,  
                                        ReasonBy, 
                                        Id, 
                                        Reason, 
                                        'registration-withdrawn-pending-confirmation'
                                        FROM Registration
                                        WHERE Reason IS NOT NULL
                                        and ReasonBy IS NOT NULL
                                        and WithdrawalStatus='PendingConfirmation'

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when ChangedDate IS NOT NULL then ChangedDate else CURRENT_TIMESTAMP end, 
                                        CommentBy, 
                                        Id, 
                                        Comment, 
                                        Case when [Status] = 'Approved' then 'registration-withdrawn-approved' else 'registration-withdrawn-rejected' end
                                        FROM Registration
                                        WHERE Comment IS NOT NULL
                                        and CommentBy IS NOT NULL
                                        and (WithdrawalStatus='Approved' or WithdrawalStatus='Rejected')

                                        INSERT INTO Comment(Id, CreatedDate, UserId, ObjectId, Content, Action)
                                        SELECT NEWID(), 
                                        Case when AdministrationDate IS NOT NULL then AdministrationDate else CURRENT_TIMESTAMP end,
                                        AdministratorCommentBy, 
                                        Id, 
                                        AdministratorComment, 
                                        Case when [Status] = 'Withdrawn' then 'registration-withdrawn-confirmed-by-ca' else 'registration-withdrawn-rejected-by-ca' end
                                        FROM Registration
                                        WHERE AdministratorComment IS NOT NULL
                                        and AdministratorCommentBy IS NOT NULL
                                        and (WithdrawalStatus='Withdrawn' or WithdrawalStatus='RejectedByCA')
                                    END");

            migrationBuilder.DropColumn(
                name: "AdministratorComment",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "AdministratorCommentBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CommentBy",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "ReasonBy",
                table: "Registration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorComment",
                table: "Registration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorCommentBy",
                table: "Registration",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Registration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentBy",
                table: "Registration",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Registration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReasonBy",
                table: "Registration",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
