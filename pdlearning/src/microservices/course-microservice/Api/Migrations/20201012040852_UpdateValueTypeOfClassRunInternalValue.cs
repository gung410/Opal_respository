using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateValueTypeOfClassRunInternalValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_CourseId_Value_Type",
                table: "CourseInternalValue");
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_Value_Type_CourseId",
                table: "CourseInternalValue");
            migrationBuilder.DropIndex(
                name: "IX_CourseInternalValue_Value",
                table: "CourseInternalValue");
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClassRunInternalValue",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_CourseId_Value_Type",
                table: "CourseInternalValue",
                columns: new[] { "CourseId", "Value", "Type" });
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_Value_Type_CourseId",
                table: "CourseInternalValue",
                columns: new[] { "Value", "Type", "CourseId" });
            migrationBuilder.CreateIndex(
                name: "IX_CourseInternalValue_Value",
                table: "CourseInternalValue",
                columns: new[] { "Value" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Value",
                table: "ClassRunInternalValue",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
