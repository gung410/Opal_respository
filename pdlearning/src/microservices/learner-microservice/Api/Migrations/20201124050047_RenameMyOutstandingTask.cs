using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class RenameMyOutstandingTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MyOutstandingTask",
                table: "MyOutstandingTask");

            migrationBuilder.RenameTable(
                name: "MyOutstandingTask",
                newName: "MyOutstandingTasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyOutstandingTasks",
                table: "MyOutstandingTasks",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MyOutstandingTasks",
                table: "MyOutstandingTasks");

            migrationBuilder.RenameTable(
                name: "MyOutstandingTasks",
                newName: "MyOutstandingTask");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyOutstandingTask",
                table: "MyOutstandingTask",
                column: "Id");
        }
    }
}
