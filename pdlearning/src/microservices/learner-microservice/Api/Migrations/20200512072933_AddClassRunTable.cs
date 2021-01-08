using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddClassRunTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassRun",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ClassRunId = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    ClassTitle = table.Column<string>(maxLength: 2000, nullable: true),
                    ClassRunCode = table.Column<string>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: true),
                    EndDateTime = table.Column<DateTime>(nullable: true),
                    PlanningStartTime = table.Column<DateTime>(nullable: true),
                    PlanningEndTime = table.Column<DateTime>(nullable: true),
                    FacilitatorIds = table.Column<string>(nullable: true),
                    CoFacilitatorIds = table.Column<string>(nullable: true),
                    ContentStatus = table.Column<string>(unicode: false, maxLength: 30, nullable: false, defaultValue: "Draft"),
                    PublishedContentDate = table.Column<DateTime>(nullable: true),
                    SubmittedContentDate = table.Column<DateTime>(nullable: true),
                    ApprovalContentDate = table.Column<DateTime>(nullable: true),
                    MinClassSize = table.Column<int>(nullable: false),
                    MaxClassSize = table.Column<int>(nullable: false),
                    ApplicationStartDate = table.Column<DateTime>(nullable: true),
                    ApplicationEndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValue: "Unpublished"),
                    ClassRunVenueId = table.Column<Guid>(nullable: true),
                    CancellationStatus = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    RescheduleStatus = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    RescheduleStartDateTime = table.Column<DateTime>(nullable: true),
                    RescheduleEndDateTime = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    ContentApprovalComment = table.Column<string>(nullable: true),
                    ContentApprovalCommentBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRun", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassRun");
        }
    }
}
