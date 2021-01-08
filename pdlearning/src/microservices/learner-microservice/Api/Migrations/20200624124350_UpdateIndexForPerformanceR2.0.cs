using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class UpdateIndexForPerformanceR20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_CreatedBy_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_CreatedBy_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_ItemId_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_ItemId_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_ItemType_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_ItemType_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_ParentCommentId_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_ParentCommentId_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_Rate_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_Rate_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_UserId_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_UserId_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_ItemId_UserId_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_ItemId_UserId_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_UserReviews_UserId_ItemId_IsDeleted_CreatedDate' AND object_id = OBJECT_ID('dbo.UserReviews'))
                                    DROP INDEX IX_UserReviews_UserId_ItemId_IsDeleted_CreatedDate ON UserReviews; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_ChangedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_ChangedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_ClassRunCode_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_ClassRunCode_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_ClassRunId_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_ClassRunId_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_ContentStatus_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_ContentStatus_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_CourseId_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_CourseId_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_CreatedBy_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_CreatedBy_CreatedDate ON ClassRun; ");

            migrationBuilder.Sql(@"IF EXISTS (
                                    SELECT 1 FROM sys.indexes
                                    WHERE name = 'IX_ClassRun_Status_CreatedDate' AND object_id = OBJECT_ID('dbo.ClassRun'))
                                    DROP INDEX IX_ClassRun_Status_CreatedDate ON ClassRun; ");

            migrationBuilder.AlterColumn<string>(
                name: "ClassRunCode",
                table: "ClassRun",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_CreatedBy_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "CreatedBy", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ItemId_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "ItemId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ItemType_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "ItemType", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ParentCommentId_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "ParentCommentId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_Rate_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "Rate", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "UserId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ItemId_UserId_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "ItemId", "UserId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId_ItemId_IsDeleted_CreatedDate",
                table: "UserReviews",
                columns: new[] { "UserId", "ItemId", "IsDeleted", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ChangedDate",
                table: "ClassRun",
                column: "ChangedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CreatedDate",
                table: "ClassRun",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ClassRunCode_CreatedDate",
                table: "ClassRun",
                columns: new[] { "ClassRunCode", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ClassRunId_CreatedDate",
                table: "ClassRun",
                columns: new[] { "ClassRunId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ContentStatus_CreatedDate",
                table: "ClassRun",
                columns: new[] { "ContentStatus", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CourseId_CreatedDate",
                table: "ClassRun",
                columns: new[] { "CourseId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CreatedBy_CreatedDate",
                table: "ClassRun",
                columns: new[] { "CreatedBy", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_Status_CreatedDate",
                table: "ClassRun",
                columns: new[] { "Status", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserReviews_CreatedBy_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_ItemId_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_ItemType_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_ParentCommentId_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_Rate_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_UserId_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_ItemId_UserId_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_UserReviews_UserId_ItemId_IsDeleted_CreatedDate",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ChangedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ClassRunCode_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ClassRunId_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ContentStatus_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CourseId_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CreatedBy_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_Status_CreatedDate",
                table: "ClassRun");

            migrationBuilder.AlterColumn<string>(
                name: "ClassRunCode",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
