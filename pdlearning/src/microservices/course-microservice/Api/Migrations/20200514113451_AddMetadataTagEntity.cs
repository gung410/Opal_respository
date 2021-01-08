using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class AddMetadataTagEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetadataTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentTagId = table.Column<Guid>(nullable: true),
                    FullStatement = table.Column<string>(nullable: true),
                    DisplayText = table.Column<string>(nullable: true),
                    GroupCode = table.Column<string>(nullable: true),
                    CodingScheme = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataTags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetadataTags_CodingScheme",
                table: "MetadataTags",
                column: "CodingScheme");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataTags_GroupCode",
                table: "MetadataTags",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataTags_ParentTagId",
                table: "MetadataTags",
                column: "ParentTagId");

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
                                                    WHERE Id = @TagId
                                                )
			                                    BEGIN
				                                    INSERT INTO [dbo].[MetadataTags] (Id, ParentTagId, FullStatement, DisplayText, GroupCode, CodingScheme, Note, [Type]) VALUES (@TagId, @ParentTagId, @FullStatement, @DisplayText, @GroupCode, @CodingScheme, @Note, @Type);
			                                    END
		                                    ELSE
			                                    BEGIN
				                                    UPDATE [dbo].[MetadataTags] SET ParentTagId = @ParentTagId, FullStatement = @FullStatement, DisplayText = @DisplayText, GroupCode = @GroupCode, CodingScheme = @CodingScheme, Note = @Note, [Type] = @Type WHERE Id = @TagId
			                                    END
	                                    END
                                    GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetadataTags");
        }
    }
}
