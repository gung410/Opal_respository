IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Staging_Resources_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Staging_Resources_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Staging_Resources_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Resources(  ID,title,repository,category,creator__id,owner__id,division,school,created_time,last_access_time,accesscount,filename, location,filesize, description,
										keywords,resourcetype,resourcesubtype,language,digitalformat,copyright,source,termsofuse,expirydate,publisher,details)
	SELECT  t1.ID,t1.title,t1.repository,t1.category,t1.creator__id,t1.owner__id,t1.division,t1.school,
			CAST(	CASE
						WHEN t1.created_time = ''
							THEN NULL
						ELSE t1.created_time
					END AS DATETIME2(7)) as created_time,
			CAST(	CASE
						WHEN t1.last_access_time = ''
							THEN NULL
						ELSE t1.last_access_time
					END AS DATETIME2(7)) as last_access_time,
			t1.accesscount, t1.filename, t1.location,t1.filesize, t1.description,
		    t1.keywords,t1.resourcetype,t1.resourcesubtype,t1.language,t1.digitalformat,t1.copyright,t1.source,t1.termsofuse,
			CAST(	CASE
						WHEN t1.expirydate = ''
							THEN NULL
						ELSE t1.expirydate
					END AS DATETIME2(7)) as expirydate,
			t1.publisher,t1.details
	FROM opa.Raw_Resources t1
	LEFT JOIN opa.Staging_Resources t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
