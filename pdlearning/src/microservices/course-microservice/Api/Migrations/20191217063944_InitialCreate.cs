using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    Version = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    CourseCode = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    CourseName = table.Column<string>(maxLength: 450, nullable: false),
                    CourseType = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    CourseContent = table.Column<string>(nullable: true),
                    CourseLevel = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    CourseObjective = table.Column<string>(nullable: true),
                    DurationMinutes = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    ThumbnailUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    TrailerVideoUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    TermsOfUse = table.Column<string>(nullable: true),
                    CourseSourceId = table.Column<Guid>(nullable: true),
                    CourseCollectionId = table.Column<Guid>(nullable: true),
                    CopyRightId = table.Column<Guid>(nullable: true),
                    IsExternalCourse = table.Column<bool>(nullable: false),
                    ExternalCourseId = table.Column<string>(unicode: false, maxLength: 450, nullable: true),
                    ExternalSourceId = table.Column<string>(unicode: false, maxLength: 450, nullable: true),
                    CourseCompletionStatus = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Status = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    ApprovingOfficer = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ApprovingOfficerComment = table.Column<string>(nullable: true),
                    IsAutoPublish = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    IsAllowDownload = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    ExternalId = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    EndOfRegistration = table.Column<DateTime>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentType = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    ThumbnailUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    Showcase = table.Column<bool>(nullable: false),
                    IsExternal = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCollectionContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    CourseCollectionId = table.Column<Guid>(nullable: false),
                    MimeType = table.Column<string>(maxLength: 450, nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    Value = table.Column<string>(unicode: false, maxLength: 2000, nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    ThumbnailUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCollectionContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseINTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseINTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lecture",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    SectionId = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    CopyRightId = table.Column<Guid>(nullable: true),
                    LectureName = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Type = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ThumbnailUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentType = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LectureContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    LectureId = table.Column<Guid>(nullable: false),
                    ResourceId = table.Column<Guid>(nullable: true),
                    MimeType = table.Column<string>(maxLength: 450, nullable: true),
                    Priority = table.Column<int>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Type = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    ThumbnailUrl = table.Column<string>(unicode: false, maxLength: 300, nullable: true),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    LockNextCardIfUncompleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    ParentSectionId = table.Column<Guid>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    SectionSeqPath = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    Level = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentType = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    DeletedDate = table.Column<DateTime>(nullable: true),
                    TagName = table.Column<string>(maxLength: 450, nullable: false),
                    Color = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "CourseCollection");

            migrationBuilder.DropTable(
                name: "CourseCollectionContent");

            migrationBuilder.DropTable(
                name: "CourseINTag");

            migrationBuilder.DropTable(
                name: "Lecture");

            migrationBuilder.DropTable(
                name: "LectureContent");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
