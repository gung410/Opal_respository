using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22_Indexes_HierarchyDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_HDID",
                table: "HierarchyDepartments",
                column: "HDID");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_ParentID",
                table: "HierarchyDepartments",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments",
                column: "Path");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_HDID",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_ParentID",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments");
        }
    }
}
