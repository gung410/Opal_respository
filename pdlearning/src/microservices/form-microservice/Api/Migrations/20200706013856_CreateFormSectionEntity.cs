using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class CreateFormSectionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormSectionId",
                table: "FormQuestions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinorPriority",
                table: "FormQuestions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    FormId = table.Column<Guid>(nullable: false),
                    MainDescription = table.Column<string>(type: "nvarchar(3000)", nullable: true),
                    AdditionalDescription = table.Column<string>(type: "nvarchar(3000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSections", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormSections");

            migrationBuilder.DropColumn(
                name: "FormSectionId",
                table: "FormQuestions");

            migrationBuilder.DropColumn(
                name: "MinorPriority",
                table: "FormQuestions");
        }
    }
}
