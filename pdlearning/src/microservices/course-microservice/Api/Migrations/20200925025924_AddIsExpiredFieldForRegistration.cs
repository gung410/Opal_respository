using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddIsExpiredFieldForRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExpired",
                table: "Registration",
                nullable: false,
                defaultValue: false);

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_IsExpired_IsDeleted_CreatedDate", "Registration");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_IsExpired_IsDeleted_CreatedDate] ON [dbo].[Registration] ([IsExpired] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "Registration");
        }
    }
}
