using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Conexus.Opal.Microservice.Tagging.Migrations
{
    public partial class InitialTaggingDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS
                                  (SELECT *
                                   FROM sysobjects
                                   WHERE name='MetadataTags' AND xtype='U')
                                   BEGIN
                                         CREATE TABLE [dbo].MetadataTags (
                                            Id INT IDENTITY PRIMARY KEY,
                                            TagId UNIQUEIDENTIFIER,
                                            ParentTagId UNIQUEIDENTIFIER,
                                            FullStatement NVARCHAR(1000) NOT NULL,
                                            DisplayText NVARCHAR(1000) NOT NULL,
                                            GroupCode NVARCHAR(100) NULL,
                                            CodingScheme NVARCHAR(100) NULL,
                                            Note NVARCHAR(4000) NULL
                                        ) AS NODE;
	                                END
                                GO


                                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_MetadataTags_TagId' AND object_id = OBJECT_ID('MetadataTags'))
                                BEGIN
	                                CREATE UNIQUE NONCLUSTERED INDEX [IX_MetadataTags_TagId] ON [dbo].[MetadataTags]
	                                (
		                                [TagId] ASC
	                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                END
                                GO

    

                                IF NOT EXISTS
                                  (SELECT *
                                   FROM sysobjects
                                   WHERE name='Resources' AND xtype='U')
                                   BEGIN
                                        CREATE TABLE [dbo].[Resources] (
                                            ResourceId UNIQUEIDENTIFIER PRIMARY KEY,
                                            ResourceType NVARCHAR(100) NOT NULL,
                                            MainSubjectAreaTagId UNIQUEIDENTIFIER NULL,
                                            PreRequisties NVARCHAR(4000) NULL,
                                            ObjectivesOutCome NVARCHAR(4000) NULL,
                                            CreatedBy UNIQUEIDENTIFIER
                                        ) AS NODE;
                                    END
                                GO


                                -- A Course can be tagged with multiple tags.
                                IF NOT EXISTS
                                  (SELECT *
                                   FROM sysobjects
                                   WHERE name='TaggedWith' AND xtype='U')
                                   BEGIN
                                        CREATE TABLE [dbo].[TaggedWith] AS EDGE;
                                   END
                                Go
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspGetMetadataTagsRecursiveByTagIds;
                                    GO
                                    CREATE PROCEDURE uspGetMetadataTagsRecursiveByTagIds
	                                    @TagIds NVARCHAR(MAX)
                                    AS
	                                    BEGIN
                                            DECLARE @DocHandle INT;
                                            DECLARE @XmlPath VARCHAR(50) = 'ArrayOfString/string';

                                            --Create an internal representation of the XML document.
                                            EXEC sp_xml_preparedocument @DocHandle OUTPUT, @TagIds;

		                                    WITH Tags (ParentTagId, TagId, Level)
                                            AS
                                            (
                                                -- Anchor member definition
                                                SELECT m.ParentTagId, m.TagId, 0 AS Level
                                                FROM MetadataTags AS m
	                                            WHERE TagId IN
                                                (
                                                    SELECT
                                                        UserTagId
                                                    FROM
                                                        OPENXML(@DocHandle, @XmlPath)
                                                        WITH
                                                        (
                                                            UserTagId UNIQUEIDENTIFIER 'text()'
                                                        )
                                                )

                                                UNION ALL

                                                -- Recursive member definition
                                                SELECT m.ParentTagId, m.TagId, Level + 1
                                                FROM Tags AS t
                                                INNER JOIN [dbo].[MetadataTags] AS m ON t.ParentTagId = m.TagId
                                            )
		                                    SELECT * FROM [dbo].[MetadataTags] MetadataTags1
		                                    WHERE MetadataTags1.TagId IN (SELECT TagId from Tags)

		                                    EXEC sp_xml_removedocument @DocHandle;
	                                    END
                                    GO
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspGetSuggestedResources;
                                    GO
                                    CREATE PROCEDURE uspGetSuggestedResources
                                        @ResourceType NVARCHAR(100)
                                        , @TagIds NVARCHAR(MAX)
                                        , @ExcludedGroupCodes NVARCHAR(MAX)
                                    AS
                                        BEGIN
                                            DECLARE @DocHandle INT;
                                            DECLARE @XmlPath VARCHAR(50) = 'ArrayOfString/string';

                                            DECLARE @DocHandle1 INT;
                                            DECLARE @XmlPath1 VARCHAR(50) = 'ArrayOfString/string';

                                            --Create an internal representation of the XML document.
                                            EXEC sp_xml_preparedocument @DocHandle OUTPUT, @TagIds;
                                            EXEC sp_xml_preparedocument @DocHandle1 OUTPUT, @ExcludedGroupCodes;

                                            WITH Tags (ParentTagId, TagId, Level)
                                            AS
                                            (
                                                -- Anchor member definition
                                                SELECT m.ParentTagId, m.TagId, 0 AS Level
                                                FROM [dbo].[MetadataTags] AS m
                                                WHERE TagId IN
                                                (
                                                    SELECT
                                                        UserTagId
                                                    FROM
                                                        OPENXML(@DocHandle, @XmlPath)
                                                        WITH
                                                        (
                                                            UserTagId UNIQUEIDENTIFIER 'text()'
                                                        )
                                                )
                                                UNION ALL

                                                -- Recursive member definition
                                                SELECT m.ParentTagId, m.TagId, Level + 1
                                                FROM Tags AS t
                                                INNER JOIN [dbo].[MetadataTags] AS m ON t.ParentTagId = m.TagId
                                            ),
                                            ExcludedGroupCodes (Code)
                                            AS
                                            (
                                                SELECT
                                                    Code
                                                FROM
                                                    OPENXML(@DocHandle1, @XmlPath1)
                                                    WITH
                                                    (
                                                        Code NVARCHAR(200) 'text()'
                                                    )
                                            )
                                            SELECT Resources1.ResourceId
                                            FROM [dbo].[Resources] Resources1, [dbo].[TaggedWith], [dbo].[MetadataTags] MetadataTags1
                                            WHERE MATCH(Resources1-(TaggedWith)->MetadataTags1)
                                            AND MetadataTags1.TagId IN (SELECT TagId from Tags)
                                            AND MetadataTags1.GroupCode NOT IN (SELECT Code from ExcludedGroupCodes)
                                            AND Resources1.ResourceType = @ResourceType

                                            EXEC sp_xml_removedocument @DocHandle;
                                            EXEC sp_xml_removedocument @DocHandle1;
                                        END
                                    GO
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspSaveMetadataTag
                                    GO

                                    CREATE PROCEDURE uspSaveMetadataTag
	                                    @TagId UNIQUEIDENTIFIER,
	                                    @ParentTagId UNIQUEIDENTIFIER,
	                                    @FullStatement NVARCHAR(1000),
                                        @DisplayText NVARCHAR(1000),
                                        @GroupCode NVARCHAR(100) NULL,
                                        @CodingScheme NVARCHAR(100) NULL,
                                        @Note NVARCHAR(4000) NULL
                                    AS
	                                    BEGIN
		                                    IF NOT EXISTS
                                                (
                                                    SELECT * FROM [dbo].[MetadataTags]
                                                    WHERE TagId = @TagId
                                                )
			                                    BEGIN
				                                    INSERT INTO [dbo].[MetadataTags] (TagId, ParentTagId, FullStatement, DisplayText, GroupCode, CodingScheme, Note) VALUES (@TagId, @ParentTagId, @FullStatement, @DisplayText, @GroupCode, @CodingScheme, @Note);
			                                    END
		                                    ELSE
			                                    BEGIN
				                                    UPDATE [dbo].[MetadataTags] SET ParentTagId = @ParentTagId, FullStatement = @FullStatement, DisplayText = @DisplayText, GroupCode = @GroupCode, CodingScheme = @CodingScheme, Note = @Note WHERE TagId = @TagId
			                                    END
	                                    END
                                    GO
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspSaveResourceMetadata;
                                    GO
                                    CREATE PROCEDURE uspSaveResourceMetadata
                                        @ResourceId UNIQUEIDENTIFIER
                                        , @ResourceType NVARCHAR(100)
                                        , @TagIds NVARCHAR(MAX)
                                        , @MainSubjectAreaTagId UNIQUEIDENTIFIER NULL
                                        , @PreRequisties NVARCHAR(4000) NULL
                                        , @ObjectivesOutCome NVARCHAR(4000) NULL
                                        , @UserId UNIQUEIDENTIFIER
                                    AS
                                        BEGIN
                                            BEGIN
                                                IF NOT EXISTS
                                                (
                                                    SELECT * FROM [dbo].[Resources]
                                                    WHERE ResourceId = @ResourceId
                                                )
                                                    BEGIN
                                                        INSERT INTO [dbo].[Resources] (ResourceId, ResourceType, MainSubjectAreaTagId, PreRequisties, ObjectivesOutCome, CreatedBy) VALUES (@ResourceId, @ResourceType, @MainSubjectAreaTagId, @PreRequisties, @ObjectivesOutCome, @UserId);
                                                    END
                                                ELSE 
				                                    BEGIN
					                                    DELETE [dbo].[TaggedWith] WHERE $from_id = (SELECT $node_id FROM [dbo].[Resources] WHERE ResourceId = @ResourceId);
					                                    UPDATE [dbo].[Resources] SET MainSubjectAreaTagId = @MainSubjectAreaTagId, PreRequisties = @PreRequisties, ObjectivesOutCome = @ObjectivesOutCome WHERE ResourceId = @ResourceId;
				                                    END
                                            END

                                            BEGIN
                                                DECLARE @DocHandle INT;
                                                DECLARE @XmlPath VARCHAR(50) = 'ArrayOfString/string';
                                                DECLARE @TagId UNIQUEIDENTIFIER;

                                                --Create an internal representation of the XML document.  
                                                EXEC sp_xml_preparedocument @DocHandle OUTPUT, @TagIds;

                                                DECLARE Cur CURSOR FOR (
                                                    SELECT
                                                        TagId
                                                    FROM
                                                        OPENXML(@DocHandle, @XmlPath)
                                                        WITH
                                                        (
                                                            TagId UNIQUEIDENTIFIER 'text()'
                                                        )
                                                );

                                                OPEN Cur
                                                FETCH NEXT FROM Cur 
                                                INTO @TagId
            
                                                -- Looping to tag identifiers to create TaggedWith relationship
                                                WHILE @@FETCH_STATUS = 0
                                                    BEGIN
                                                        INSERT INTO [dbo].[TaggedWith] ($from_id, $to_id) VALUES (
                                                            (SELECT $node_id FROM [dbo].[Resources] WHERE ResourceId = @ResourceId), 
                                                            (SELECT $node_id FROM [dbo].[MetadataTags] WHERE TagId = @TagId))

                                                        FETCH NEXT FROM Cur
                                                        INTO @TagId
                                                    END

                                                CLOSE Cur;
                                                DEALLOCATE Cur;
                                                EXEC sp_xml_removedocument @DocHandle;
                                            END
                                        END
                                    GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Todo: add migration scriptings down
        }
    }
}
