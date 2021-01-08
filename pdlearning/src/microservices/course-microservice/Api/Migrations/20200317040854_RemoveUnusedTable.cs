using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveUnusedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate Data before removing
            migrationBuilder.Sql("UPDATE Course SET AllowPersonalDownload=IsAllowDownload");

            migrationBuilder.Sql("UPDATE Course SET AllowNonCommerInMOEReuseWithModification = 1 WHERE IsAllowReusable = 1 and IsAllowModification = 1");

            migrationBuilder.Sql("UPDATE Course SET AllowNonCommerInMoeReuseWithoutModification =1 WHERE IsAllowReusable = 1 AND IsAllowModification = 0");

            migrationBuilder.DropTable(
                name: "AttributionElements");

            migrationBuilder.DropTable(
                name: "CourseCollection");

            migrationBuilder.DropTable(
                name: "CourseCollectionContent");

            migrationBuilder.DropTable(
                name: "CourseINTag");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropColumn(
                name: "ApprovingOfficer",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ApprovingOfficerComment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "AttributionUrl",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CopyRightId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseCollectionId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseCompletionStatus",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "CourseSourceId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "EndOfRegistration",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ExternalCourseId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ExternalSourceId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAllowDownload",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAllowModification",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAllowReusable",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsAutoPublish",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsExternalCourse",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LicenseTerritory",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "LicenseType",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Ownership",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TrailerVideoUrl",
                table: "Course");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovingOfficer",
                table: "Course",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovingOfficerComment",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttributionUrl",
                table: "Course",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CopyRightId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "Course",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseCollectionId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseCompletionStatus",
                table: "Course",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseSourceId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Course",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndOfRegistration",
                table: "Course",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalCourseId",
                table: "Course",
                type: "varchar(450)",
                unicode: false,
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSourceId",
                table: "Course",
                type: "varchar(450)",
                unicode: false,
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowDownload",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowModification",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowReusable",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoPublish",
                table: "Course",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))");

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalCourse",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseTerritory",
                table: "Course",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "Course",
                type: "varchar(36)",
                unicode: false,
                maxLength: 36,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ownership",
                table: "Course",
                type: "varchar(40)",
                unicode: false,
                maxLength: 40,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentType",
                table: "Course",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Course",
                type: "decimal(18, 2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Course",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerVideoUrl",
                table: "Course",
                type: "varchar(300)",
                unicode: false,
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttributionElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LicenseType = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributionElements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Showcase = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCollectionContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseCollectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    Title = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Value = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCollectionContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseINTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseINTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    TagName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });
        }
    }
}
