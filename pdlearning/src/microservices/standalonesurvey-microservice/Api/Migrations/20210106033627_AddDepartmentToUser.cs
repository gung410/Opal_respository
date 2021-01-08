using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.StandaloneSurvey.Migrations
{
    public partial class AddDepartmentToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "Users");
        }
    }
}
