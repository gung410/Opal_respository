using Microsoft.EntityFrameworkCore.Migrations;

namespace Conexus.Opal.Microservice.Tagging.Migrations
{
    public partial class AddTypeColumnForMetadataTagEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE [dbo].[MetadataTags]
                                    ADD [Type] NVARCHAR(100)
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
                                        @Note NVARCHAR(4000) NULL,
				                        @Type NVARCHAR(100) NULL
                                    AS
	                                    BEGIN
		                                    IF NOT EXISTS
                                                (
                                                    SELECT * FROM [dbo].[MetadataTags]
                                                    WHERE TagId = @TagId
                                                )
			                                    BEGIN
				                                    INSERT INTO [dbo].[MetadataTags] (TagId, ParentTagId, FullStatement, DisplayText, GroupCode, CodingScheme, Note, [Type]) VALUES (@TagId, @ParentTagId, @FullStatement, @DisplayText, @GroupCode, @CodingScheme, @Note, @Type);
			                                    END
		                                    ELSE
			                                    BEGIN
				                                    UPDATE [dbo].[MetadataTags] SET ParentTagId = @ParentTagId, FullStatement = @FullStatement, DisplayText = @DisplayText, GroupCode = @GroupCode, CodingScheme = @CodingScheme, Note = @Note, [Type] = @Type WHERE TagId = @TagId
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
