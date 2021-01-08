using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddUserReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ParentCommentId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemType = table.Column<string>(type: "varchar(30)", nullable: false),
                    Version = table.Column<string>(type: "varchar(100)", nullable: true),
                    ItemName = table.Column<string>(maxLength: 500, nullable: true),
                    UserFullName = table.Column<string>(maxLength: 500, nullable: true),
                    CommentTitle = table.Column<string>(maxLength: 100, nullable: true),
                    CommentContent = table.Column<string>(maxLength: 2000, nullable: true),
                    Rate = table.Column<double>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReviews");
        }
    }
}
