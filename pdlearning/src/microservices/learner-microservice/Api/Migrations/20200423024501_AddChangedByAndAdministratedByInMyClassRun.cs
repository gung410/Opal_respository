using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddChangedByAndAdministratedByInMyClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdministratedBy",
                table: "MyClassRun",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "MyClassRun",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministratedBy",
                table: "MyClassRun");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "MyClassRun");
        }
    }
}
