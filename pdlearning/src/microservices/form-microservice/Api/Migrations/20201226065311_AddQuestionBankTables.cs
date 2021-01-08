using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddQuestionBankTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionBanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionTitle = table.Column<string>(type: "NTEXT", maxLength: 20000, nullable: true),
                    QuestionType = table.Column<string>(type: "varchar(30)", nullable: false),
                    QuestionCorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionHint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionAnswerExplanatoryNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionFeedbackCorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionFeedbackWrongAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionLevel = table.Column<int>(type: "int", nullable: true),
                    RandomizedOptions = table.Column<bool>(type: "bit", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsScoreEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBanks", x => x.Id);
                });
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_QuestionBanks]WITH ACCENT_SENSITIVITY = ON", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.QuestionBanks(Title) KEY INDEX [PK_QuestionBanks] ON [FTS_QuestionBanks] WITH (STOPLIST=OFF)", true);

            migrationBuilder.CreateTable(
                name: "QuestionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionGroups", x => x.Id);
                });
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_QuestionGroups]WITH ACCENT_SENSITIVITY = ON", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.QuestionGroups(Name) KEY INDEX [PK_QuestionGroups] ON [FTS_QuestionGroups] WITH (STOPLIST=OFF)", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @$"IF EXISTS (select 1 from sys.fulltext_indexes
                join sys.objects on fulltext_indexes.object_id = objects.object_id where objects.name = 'QuestionBanks')
                DROP FULLTEXT INDEX ON dbo.QuestionBanks",
                true);
            migrationBuilder.Sql(
                @$"IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'FTS_QuestionBanks')
                BEGIN
                    DROP FULLTEXT CATALOG FTS_QuestionBanks
                END",
                true);
            migrationBuilder.Sql(
                @$"IF EXISTS (select 1 from sys.fulltext_indexes
                join sys.objects on fulltext_indexes.object_id = objects.object_id where objects.name = 'QuestionGroups')
                DROP FULLTEXT INDEX ON dbo.QuestionGroups",
                true);
            migrationBuilder.Sql(
                @$"IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'FTS_QuestionGroups')
                BEGIN
                    DROP FULLTEXT CATALOG FTS_QuestionGroups
                END",
                true);
            migrationBuilder.DropTable(
                name: "QuestionBanks");

            migrationBuilder.DropTable(
                name: "QuestionGroups");
        }
    }
}
