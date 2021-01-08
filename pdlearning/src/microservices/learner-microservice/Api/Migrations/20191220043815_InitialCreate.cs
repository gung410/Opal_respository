using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    ParentCommentId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(type: "varchar(100)", nullable: true),
                    SectionId = table.Column<Guid>(nullable: true),
                    LectureId = table.Column<Guid>(nullable: true),
                    ItemName = table.Column<string>(maxLength: 500, nullable: true),
                    UserFullName = table.Column<string>(maxLength: 500, nullable: true),
                    CommentTitle = table.Column<string>(maxLength: 100, nullable: true),
                    CommentContent = table.Column<string>(maxLength: 2000, nullable: true),
                    Rate = table.Column<double>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LecturesInMyCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    MyCourseId = table.Column<Guid>(nullable: false),
                    LectureId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    ReviewStatus = table.Column<string>(maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    LastLogin = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturesInMyCourse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: false),
                    Version = table.Column<string>(type: "varchar(100)", nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    ReviewStatus = table.Column<string>(maxLength: 1000, nullable: true),
                    ProgressMeasure = table.Column<double>(nullable: true),
                    LastLogin = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DisenrollUtc = table.Column<DateTime>(nullable: true),
                    ReadDate = table.Column<DateTime>(nullable: true),
                    ReminderSentDate = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    CompletedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyCourses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyLearningPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    MyLectureId = table.Column<Guid>(nullable: true),
                    MyDigitalContentId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(type: "varchar(30)", nullable: false),
                    State = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyLearningPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBookmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    ItemType = table.Column<string>(type: "varchar(30)", nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 100, nullable: true),
                    Comment = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ChangedDate = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Gender = table.Column<bool>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    PlaceOfWork = table.Column<string>(nullable: true),
                    TeachingLevel = table.Column<string>(type: "varchar(30)", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseReviews");

            migrationBuilder.DropTable(
                name: "LecturesInMyCourse");

            migrationBuilder.DropTable(
                name: "MyCourses");

            migrationBuilder.DropTable(
                name: "MyLearningPackages");

            migrationBuilder.DropTable(
                name: "UserBookmarks");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
