using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class RemoveSoftDeleteForRegistrationECertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationECertificate_UserId_IsDeleted_CreatedDate",
                table: "RegistrationECertificate");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RegistrationECertificate");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationECertificate_UserId_CreatedDate",
                table: "RegistrationECertificate",
                columns: new[] { "UserId", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationECertificate_UserId_CreatedDate",
                table: "RegistrationECertificate");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RegistrationECertificate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationECertificate_UserId_IsDeleted_CreatedDate",
                table: "RegistrationECertificate",
                columns: new[] { "UserId", "IsDeleted", "CreatedDate" });
        }
    }
}
