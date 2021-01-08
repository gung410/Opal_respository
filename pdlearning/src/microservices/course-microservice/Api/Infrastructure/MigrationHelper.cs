using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Infrastructure
{
    public static class MigrationHelper
    {
        public static void DropFullTextIndexIfExists(MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.Sql(
                @$"IF EXISTS (select 1 from sys.fulltext_indexes
                join sys.objects on fulltext_indexes.object_id = objects.object_id where objects.name = '{tableName}')
                DROP FULLTEXT INDEX ON dbo.{tableName}",
                true);
        }

        public static void DropFullTextCatalogIfExists(MigrationBuilder migrationBuilder, string catalogName)
        {
            migrationBuilder.Sql(
                @$"IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = '{catalogName}')
                BEGIN
                    DROP FULLTEXT CATALOG {catalogName}
                END",
                true);
        }

        public static void DropIndexIfExists(MigrationBuilder migrationBuilder, string indexName, string tableName)
        {
            migrationBuilder.Sql(
                @$"IF EXISTS (
                SELECT 1 FROM sys.indexes
                WHERE name = '{indexName}' AND object_id = OBJECT_ID('dbo.{tableName}'))
                DROP INDEX {indexName} ON {tableName}; ",
                true);
        }

        public static void CreateIndex(MigrationBuilder migrationBuilder, string tableName, params string[] cols)
        {
            migrationBuilder.Sql(
                @$"CREATE NONCLUSTERED INDEX [IX_{tableName}_{string.Join("_", cols)}] ON [dbo].[{tableName}] ({string.Join(",", cols.Select(col => $"[{col}] ASC"))})",
                true);
        }

        public static void CreateUniqueIndex(MigrationBuilder migrationBuilder, string tableName, params string[] cols)
        {
            migrationBuilder.Sql(
                @$"CREATE UNIQUE NONCLUSTERED INDEX [IX_{tableName}_{string.Join("_", cols)}] ON [dbo].[{tableName}] ({string.Join(",", cols.Select(col => $"[{col}] ASC"))})",
                true);
        }
    }
}
