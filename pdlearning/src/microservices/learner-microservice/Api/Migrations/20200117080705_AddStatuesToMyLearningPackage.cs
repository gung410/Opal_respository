using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddStatuesToMyLearningPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionStatus",
                table: "MyLearningPackages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuccessStatus",
                table: "MyLearningPackages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionStatus",
                table: "MyLearningPackages");

            migrationBuilder.DropColumn(
                name: "SuccessStatus",
                table: "MyLearningPackages");
        }
    }
}
