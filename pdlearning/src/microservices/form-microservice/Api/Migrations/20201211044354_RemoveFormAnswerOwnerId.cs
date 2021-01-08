using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class RemoveFormAnswerOwnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormAnswers_OwnerId_IsDeleted_CreatedDate",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "FormAnswers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "FormAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FormAnswers_OwnerId_IsDeleted_CreatedDate",
                table: "FormAnswers",
                columns: new[] { "OwnerId", "IsDeleted", "CreatedDate" });
        }
    }
}
