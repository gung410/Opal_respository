using Microservice.Course.Common.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateFTSKeyColumnForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            F.ExecuteByPassException(() => migrationBuilder.Sql(
                "IF EXISTS (select 1 from sys.fulltext_indexes join sys.objects on fulltext_indexes.object_id = objects.object_id where objects.name = 'Users') " +
                "DROP FULLTEXT INDEX ON dbo.Users",
                true));

            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearchKey",
                table: "Users",
                unicode: false,
                maxLength: 250,
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldDefaultValue: string.Empty);

            migrationBuilder.Sql("UPDATE dbo.Users SET FullTextSearchKey = CONVERT(varchar(200), COALESCE(Email, '')) + '_' + CONVERT(varchar(40), Id)");

            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.Users(FullTextSearch) KEY INDEX IX_Users_FullTextSearchKey ON FTSUsers WITH (STOPLIST=OFF);", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullTextSearchKey",
                table: "Users",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 250,
                oldDefaultValue: string.Empty);
        }
    }
}
