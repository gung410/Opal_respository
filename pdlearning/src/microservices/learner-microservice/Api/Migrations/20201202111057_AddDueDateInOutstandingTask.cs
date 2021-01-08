﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Learner.Migrations
{
    public partial class AddDueDateInOutstandingTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "MyOutstandingTasks",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "MyOutstandingTasks");
        }
    }
}
