IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_DigitalContents_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_DigitalContents_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_DigitalContents_Insert
AS
BEGIN
	INSERT INTO opa.Migration_DigitalContents(	Id,CreatedDate,ChangedDate,Title,Description,Type,Status,IsDeleted,ChangedBy,CreatedBy,
												Discriminator,HtmlContent,FileName,FileType,FileExtension,FileSize,ExternalId,OwnerId,FileLocation,RepositoryName,ExpiredDate,
												Copyright, Publisher, Source, TermsOfUse)
	SELECT	NEWID() Id, CAST(t1.created_time AS DATETIME2(7)) CreatedDate,  CAST(t1.created_time AS DATETIME2(7)) ChangedDate, t1.title, CAST(t1.description AS NTEXT), 
			'UploadedContent' Type,'Draft' Status, '0' IsDeleted, t3.ExtID ChangedBy, t3.ExtID CreatedBy,'UploadedContent' Discriminator, NULL HtmlContent, CAST(t1.filename AS NVARCHAR(255)) FileName, 
			t1.digitalformat FileType, 
			REVERSE(SUBSTRING(REVERSE(t1.filename), 1, CHARINDEX('.',REVERSE(t1.filename)) -1) ) FileExtension, 
			t1.filesize FileSize, t1.id ExternalId, t4.ExtID OwnerId, CAST(t1.location AS NVARCHAR(255)) FileLocation, CAST(t1.repository_name AS NVARCHAR(100)), CAST(t1.expirydate  AS DATETIME2(7)),
			CAST(t1.copyright AS NVARCHAR(100)), CAST(t1.publisher AS NVARCHAR(100)), CAST(t1.source AS NVARCHAR(255)), CAST(t1.termsofuse AS NVARCHAR(4000))
	FROM opa.Staging_Resources t1
	LEFT JOIN opa.Migration_DigitalContents t2 ON t1.ID = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Staging_User t4 ON t1.owner__id = t4.ID
	WHERE t2.ExternalId IS NULL


END
GO