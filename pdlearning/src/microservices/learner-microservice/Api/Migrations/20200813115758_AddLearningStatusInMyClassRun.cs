using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddLearningStatusInMyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LearningStatus",
                table: "MyClassRun",
                unicode: false,
                maxLength: 50,
                nullable: true,
                defaultValue: "NotStarted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningStatus",
                table: "MyClassRun");
        }
    }
}
