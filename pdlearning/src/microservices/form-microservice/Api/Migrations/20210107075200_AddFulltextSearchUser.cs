using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddFulltextSearchUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullTextSearch",
                table: "Users",
                maxLength: 2100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullTextSearchKey",
                table: "Users",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.Sql("UPDATE dbo.Users SET FullTextSearch = COALESCE(FirstName, '') + '  ' + COALESCE(LastName, '') + '  ' + COALESCE(Email, '')");
            migrationBuilder.Sql("UPDATE dbo.Users SET FullTextSearchKey = CONVERT(varchar(60), ISNULL(Email,'')) + '_' + CONVERT(varchar(40), Id)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullTextSearchKey",
                table: "Users",
                column: "FullTextSearchKey",
                unique: true);

            migrationBuilder.Sql(
                @$"IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'FTSUsers')
                BEGIN
                    DROP FULLTEXT CATALOG FTSUsers
                END",
                true);

            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTSUsers]", true);

            migrationBuilder.Sql(@"CREATE FULLTEXT INDEX ON dbo.Users(FullTextSearch) KEY INDEX IX_Users_FullTextSearchKey ON FTSUsers WITH (STOPLIST=OFF);", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
               name: "IX_Users_FullTextSearchKey",
               table: "Users");

            migrationBuilder.DropColumn(
                name: "FullTextSearch",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullTextSearchKey",
                table: "Users");
        }
    }
}
