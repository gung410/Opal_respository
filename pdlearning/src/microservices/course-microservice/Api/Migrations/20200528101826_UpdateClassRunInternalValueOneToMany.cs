using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateClassRunInternalValueOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassRunInternalValue",
                table: "ClassRunInternalValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassRunInternalValue",
                table: "ClassRunInternalValue",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassRunInternalValue",
                table: "ClassRunInternalValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassRunInternalValue",
                table: "ClassRunInternalValue",
                columns: new[] { "ClassRunId", "Id" });
        }
    }
}
