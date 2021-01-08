using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveNotUsedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "ParentSectionId",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "SectionSeqPath",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Section");

            migrationBuilder.DropColumn(
                name: "LockNextCardIfUncompleted",
                table: "LectureContent");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "LectureContent");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "LectureContent");

            migrationBuilder.DropColumn(
                name: "CopyRightId",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Lecture");

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionId",
                table: "Lecture",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Section",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Section",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentSectionId",
                table: "Section",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentType",
                table: "Section",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Section",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SectionSeqPath",
                table: "Section",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Section",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LockNextCardIfUncompleted",
                table: "LectureContent",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "LectureContent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "LectureContent",
                type: "varchar(300)",
                unicode: false,
                maxLength: 300,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionId",
                table: "Lecture",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CopyRightId",
                table: "Lecture",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Lecture",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentType",
                table: "Lecture",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Lecture",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Lecture",
                type: "varchar(300)",
                unicode: false,
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Lecture",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);
        }
    }
}
