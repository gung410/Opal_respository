IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Staging_WallPost_UpDate' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Staging_WallPost_UpDate] AS ')
GO

ALTER PROCEDURE [opa].[prc_CSL_Staging_WallPost_UpDate]
AS
BEGIN

	UPDATE opa.Staging_WallPost
	SET filename = RIGHT(res_slot, CHARINDEX('/',REVERSE(res_slot))-1)
	WHERE type = 'resource';

END