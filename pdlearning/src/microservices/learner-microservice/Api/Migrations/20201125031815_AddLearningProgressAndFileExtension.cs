using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddLearningProgressAndFileExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LearningContentProgress",
                table: "MyClassRun",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "DigitalContents",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningContentProgress",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "DigitalContents");
        }
    }
}
