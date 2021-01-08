using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateRegistrationMethodForCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RegistrationMethod",
                table: "Course",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "RegistrationOrNomination");

            migrationBuilder.Sql(@"Update Course Set RegistrationMethod='Restricted' WHERE RegistrationMethod='RegistrationOrNomination'
                                  Update Course Set RegistrationMethod='Public' WHERE RegistrationMethod='RegistrationOnly'
                                  Update Course Set RegistrationMethod='Private' WHERE RegistrationMethod='NominationOnly'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Update Course Set RegistrationMethod='RegistrationOrNomination' WHERE RegistrationMethod='Restricted' OR RegistrationMethod is null
                                  Update Course Set RegistrationMethod='RegistrationOnly' WHERE RegistrationMethod='Public'
                                  Update Course Set RegistrationMethod='NominationOnly' WHERE RegistrationMethod='Private'");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationMethod",
                table: "Course",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "RegistrationOrNomination",
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
