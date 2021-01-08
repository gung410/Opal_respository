using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddDigitalContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyDigitalContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    DigitalContentId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false),
                    DigitalContentType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Version = table.Column<string>(type: "varchar(100)", nullable: true),
                    ReviewStatus = table.Column<string>(maxLength: 1000, nullable: true),
                    ProgressMeasure = table.Column<double>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DisenrollUtc = table.Column<DateTime>(nullable: true),
                    ReadDate = table.Column<DateTime>(nullable: true),
                    ReminderSentDate = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    CompletedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDigitalContent", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyDigitalContent");
        }
    }
}
