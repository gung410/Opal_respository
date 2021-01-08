using Microsoft.EntityFrameworkCore.Migrations;

namespace Conexus.Opal.Microservice.Tagging.Migrations
{
    public partial class SupportDynamicMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS
                                  (SELECT *
                                   FROM sysobjects
                                   WHERE name='ResourceDynamicMetadata' AND xtype='U')
                                   BEGIN
                                         CREATE TABLE [dbo].ResourceDynamicMetadata (
                                            ResourceId UNIQUEIDENTIFIER NOT NULL,
                                            [Key] VARCHAR(500) NOT NULL,
                                            JsonValue NVARCHAR(MAX) NOT NULL
                                        );
	                                END
                                GO


                                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ResourceDynamicMetadata_Key_ResourceId' AND object_id = OBJECT_ID('ResourceDynamicMetadata'))
                                BEGIN
	                                CREATE UNIQUE CLUSTERED INDEX [IX_ResourceDynamicMetadata_Key_ResourceId] ON [dbo].[ResourceDynamicMetadata]
	                                (
                                        [Key] ASC,
                                        [ResourceId] ASC
	                                )  ON [PRIMARY]
                                END
                                GO

                                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ResourceDynamicMetadata_ResourceId' AND object_id = OBJECT_ID('ResourceDynamicMetadata'))
                                BEGIN
	                                CREATE NONCLUSTERED INDEX [IX_ResourceDynamicMetadata_ResourceId] ON [dbo].[ResourceDynamicMetadata]
                                    (
	                                    [ResourceId] ASC
                                    )
                                    INCLUDE ( 	[Key],
	                                    [JsonValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                END
                                GO
            ");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS uspSaveResourceMetadata;
                                    GO
                                    CREATE PROCEDURE uspSaveResourceMetadata
                                        @ResourceId UNIQUEIDENTIFIER
                                        , @ResourceType NVARCHAR(100)
                                        , @TagIds NVARCHAR(MAX)
	                                    , @DynamicMetaDataItemsJson NVARCHAR(MAX)
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

		                                    BEGIN
		                                    DELETE [dbo].[ResourceDynamicMetadata] WHERE ResourceId=@ResourceId

		                                    INSERT INTO [dbo].[ResourceDynamicMetadata]
		                                    SELECT item.*
		                                    FROM OPENJSON(@DynamicMetaDataItemsJson) items
		                                    CROSS APPLY OPENJSON(items.value)
		                                    WITH
		                                    (
				                                    ResourceId UNIQUEIDENTIFIER,
				                                    [Key] VARCHAR(500),
				                                    JsonValue NVARCHAR(MAX)	
		                                    ) item

		                                    END
                                        END
                                    GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
