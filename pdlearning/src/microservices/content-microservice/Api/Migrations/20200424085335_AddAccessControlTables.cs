﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class AddAccessControlTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "DigitalContents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentTypeDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DepartmentTypeID = table.Column<int>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTypeDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DepartmentTypeID = table.Column<int>(nullable: false),
                    ExtID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HierarchyDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HDID = table.Column<int>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false),
                    ParentID = table.Column<int>(nullable: true),
                    Path = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HierarchyDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    DepartmentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "DepartmentTypeDepartments");

            migrationBuilder.DropTable(
                name: "DepartmentTypes");

            migrationBuilder.DropTable(
                name: "HierarchyDepartments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "DigitalContents");
        }
    }
}
