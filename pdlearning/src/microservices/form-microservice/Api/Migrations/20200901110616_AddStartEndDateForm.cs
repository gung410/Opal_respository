﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddStartEndDateForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Forms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Forms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Forms");
        }
    }
}
