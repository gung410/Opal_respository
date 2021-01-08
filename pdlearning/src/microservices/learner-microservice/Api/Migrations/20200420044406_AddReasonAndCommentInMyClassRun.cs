using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddReasonAndCommentInMyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MyClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MyClassRun");
        }
    }
}
