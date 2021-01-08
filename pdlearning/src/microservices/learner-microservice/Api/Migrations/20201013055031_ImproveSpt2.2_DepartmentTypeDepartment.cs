using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class ImproveSpt22_DepartmentTypeDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments");

            migrationBuilder.AlterColumn<string>(
                name: "ExtID",
                table: "DepartmentTypes",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_Path_DepartmentID",
                table: "HierarchyDepartments",
                columns: new[] { "Path", "DepartmentID" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypes_DepartmentTypeID_ExtID",
                table: "DepartmentTypes",
                columns: new[] { "DepartmentTypeID", "ExtID" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypes_ExtID_DepartmentTypeID",
                table: "DepartmentTypes",
                columns: new[] { "ExtID", "DepartmentTypeID" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentID_DepartmentTypeID",
                table: "DepartmentTypeDepartments",
                columns: new[] { "DepartmentID", "DepartmentTypeID" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentTypeID_DepartmentID",
                table: "DepartmentTypeDepartments",
                columns: new[] { "DepartmentTypeID", "DepartmentID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_Path_DepartmentID",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypes_DepartmentTypeID_ExtID",
                table: "DepartmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypes_ExtID_DepartmentTypeID",
                table: "DepartmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentID_DepartmentTypeID",
                table: "DepartmentTypeDepartments");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentTypeID_DepartmentID",
                table: "DepartmentTypeDepartments");

            migrationBuilder.AlterColumn<string>(
                name: "ExtID",
                table: "DepartmentTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments",
                column: "Path");
        }
    }
}
