using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddRegistrationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    ClassRunId = table.Column<Guid>(nullable: false),
                    RegistrationType = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    LastStatusChangedDate = table.Column<DateTime>(nullable: true),
                    WithdrawalStatus = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    WithdrawalRequestDate = table.Column<DateTime>(nullable: true),
                    ClassRunChangeStatus = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    ClassRunChangeRequestedDate = table.Column<DateTime>(nullable: true),
                    ClassRunChangeId = table.Column<Guid>(nullable: true),
                    ApprovingOfficer = table.Column<Guid>(nullable: false),
                    AlternativeApprovingOfficer = table.Column<Guid>(nullable: true),
                    ApprovingDate = table.Column<DateTime>(nullable: true),
                    AdministratedBy = table.Column<Guid>(nullable: true),
                    AdministrationDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registration");
        }
    }
}
