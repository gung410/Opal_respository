using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DigitalContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Status = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    ExternalId = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    RepositoryName = table.Column<string>(maxLength: 100, nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    HtmlContent = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(maxLength: 255, nullable: true),
                    FileType = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    FileExtension = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    FileSize = table.Column<double>(nullable: true),
                    FileLocation = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalContents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DigitalContents");
        }
    }
}
