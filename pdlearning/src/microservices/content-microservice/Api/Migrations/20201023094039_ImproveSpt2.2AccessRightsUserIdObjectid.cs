using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Content.Migrations
{
    public partial class ImproveSpt22AccessRightsUserIdObjectid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX If EXISTS IX_AccessRights_UserId_ObjectId ON dbo.AccessRights", true);
            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_UserId_ObjectId",
                table: "AccessRights",
                columns: new[] { "UserId", "ObjectId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccessRights_UserId_ObjectId",
                table: "AccessRights");
        }
    }
}
