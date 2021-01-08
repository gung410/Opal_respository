using Microsoft.EntityFrameworkCore.Migrations;

namespace Conexus.Opal.Microservice.Tagging.Migrations
{
    public partial class AddSearchTagsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS
                                    (SELECT *
                                    FROM sysobjects
                                    WHERE name='SearchTags' AND xtype='U')
                                    BEGIN
                                            CREATE TABLE [dbo].SearchTags (
			                                [Id] [uniqueidentifier] NOT NULL,
                                            [Name] NVARCHAR(MAX) NOT NULL
			                                 CONSTRAINT [PK_SearchTags] PRIMARY KEY CLUSTERED
			                                (
				                                [Id] ASC
			                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			                                ) ON [PRIMARY]
	                                END
                                GO

                                IF NOT EXISTS
                                    (SELECT *
                                    FROM sysobjects
                                    WHERE name='ResourceSearchTags' AND xtype='U')
                                    BEGIN
                                            CREATE TABLE [dbo].ResourceSearchTags (
			                                ResourceId [uniqueidentifier] NOT NULL,
			                                SearchTagId [uniqueidentifier] NOT NULL,
			                                  CONSTRAINT ResourceSearchTags_Unique UNIQUE(ResourceId,SearchTagId))
	                                END
                                GO");
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [FTS_SearchTags]WITH ACCENT_SENSITIVITY = ON", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON dbo.SearchTags(Name) KEY INDEX [PK_SearchTags] ON [FTS_SearchTags] WITH (STOPLIST=OFF)", true);
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspSaveSearchTag;
                                    GO
                                    CREATE PROCEDURE uspSaveSearchTag
                                        @ResourceId UNIQUEIDENTIFIER,
	                                    @SearchTagNames NVARCHAR(MAX)
                                    AS
	                                    BEGIN
                                            DELETE [dbo].ResourceSearchTags WHERE ResourceId = @ResourceId
		                                    DECLARE @DocHandle INT;
		                                    DECLARE @XmlPath VARCHAR(50) = 'ArrayOfString/string';
		                                    DECLARE @SearchTagName NVARCHAR(MAX);

		                                    --Create an internal representation of the XML document.  
		                                    EXEC sp_xml_preparedocument @DocHandle OUTPUT, @SearchTagNames;

		                                    DECLARE Cur CURSOR FOR (
			                                    SELECT
				                                    SearchTagName
			                                    FROM
				                                    OPENXML(@DocHandle, @XmlPath)
				                                    WITH
				                                    (
					                                    SearchTagName NVARCHAR(MAX) 'text()'
				                                    )
		                                    );

		                                    OPEN Cur
		                                    FETCH NEXT FROM Cur 
		                                    INTO @SearchTagName
            
		                                    -- Looping to tag identifiers to create TaggedWith relationship
		                                    WHILE @@FETCH_STATUS = 0
			                                    BEGIN
				                                    DECLARE @NewSearchTagId UNIQUEIDENTIFIER

				                                    IF EXISTS (SELECT * FROM [dbo].SearchTags WHERE [Name] = @SearchTagname)
					                                    BEGIN
						                                    INSERT INTO [dbo].ResourceSearchTags VALUES (@ResourceId,(SELECT TOP 1 Id FROM [dbo].SearchTags WHERE [Name] = @SearchTagname))
					                                    END
				                                    ELSE
					                                    BEGIN
						                                    SET @NewSearchTagId = NEWID()
						                                    INSERT INTO [dbo].SearchTags VALUES (@NewSearchTagId, @SearchTagname)
						                                    INSERT INTO [dbo].ResourceSearchTags VALUES (@ResourceId, @NewSearchTagId)
					                                    END
				                                    FETCH NEXT FROM Cur
				                                    INTO @SearchTagName
			                                    END
		                                    CLOSE Cur;
		                                    DEALLOCATE Cur;
		                                    EXEC sp_xml_removedocument @DocHandle;

		                                    DELETE FROM [dbo].SearchTags WHERE Id NOT IN (SELECT DISTINCT SearchTagId FROM [dbo].ResourceSearchTags)
	                                    END
                                    GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS uspSaveSearchTag");
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON dbo.SearchTags", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG FTS_SearchTags", true);
            migrationBuilder.DropTable(
                name: "ResourceSearchTags");
            migrationBuilder.DropTable(
                name: "SearchTags");
        }
    }
}
