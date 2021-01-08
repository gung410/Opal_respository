using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddCommentViewPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentViewPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    CommentAction = table.Column<string>(nullable: true),
                    CommentByUserRole = table.Column<string>(nullable: true),
                    CanViewRole = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentViewPermission", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentViewPermission_CanViewRole",
                table: "CommentViewPermission",
                column: "CanViewRole");

            migrationBuilder.CreateIndex(
                name: "IX_CommentViewPermission_ChangedDate",
                table: "CommentViewPermission",
                column: "ChangedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommentViewPermission_CommentAction",
                table: "CommentViewPermission",
                column: "CommentAction");

            migrationBuilder.CreateIndex(
                name: "IX_CommentViewPermission_CommentByUserRole",
                table: "CommentViewPermission",
                column: "CommentByUserRole");

            migrationBuilder.CreateIndex(
                name: "IX_CommentViewPermission_CreatedDate",
                table: "CommentViewPermission",
                column: "CreatedDate");

            migrationBuilder.Sql(@"
            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-reply'
           ,'Course Content Creator'
           ,'Course Planning Coordinator')
            GO

            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-reply'
           ,'Course Planning Coordinator'
           ,'Course Planning Coordinator')
            GO

            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-approved'
           ,'Course Content Creator'
           ,'Course Planning Coordinator')
            GO

            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-approved'
           ,'Course Planning Coordinator'
           ,'Course Planning Coordinator')
            GO

            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-rejected'
           ,'Course Content Creator'
           ,'Course Planning Coordinator')
            GO

            INSERT INTO [dbo].[CommentViewPermission]
           ([ChangedDate]
           ,[CommentAction]
           ,[CommentByUserRole]
           ,[CanViewRole])
            VALUES
           (NULL
           ,'course-rejected'
           ,'Course Planning Coordinator'
           ,'Course Planning Coordinator')
            GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentViewPermission");
        }
    }
}
