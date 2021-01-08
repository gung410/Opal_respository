using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddMyWithdrawalStatusAtMyCourseAndMyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MyWithdrawalStatus",
                table: "MyCourses",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawalStatus",
                table: "MyClassRun",
                type: "varchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyWithdrawalStatus",
                table: "MyCourses");

            migrationBuilder.DropColumn(
                name: "WithdrawalStatus",
                table: "MyClassRun");
        }
    }
}
