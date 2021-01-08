using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateRegistrationECertificateRelationShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationECertificate_Registration_Id",
                table: "RegistrationECertificate",
                column: "Id",
                principalTable: "Registration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationECertificate_Registration_Id",
                table: "RegistrationECertificate");
        }
    }
}
