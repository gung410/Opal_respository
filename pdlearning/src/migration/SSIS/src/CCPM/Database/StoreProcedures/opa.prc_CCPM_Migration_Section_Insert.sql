IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_Section_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_Section_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_Section_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Section (	Id,CreatedDate,ChangedDate,DeletedDate,ParentSectionId,CourseId,Version,Title,Description,IsDeleted,
										Priority,SectionSeqPath,Level,CreatedBy,ChangedBy,ParentId,ParentType,[Order], ExternalId )
	SELECT NEWID() Id, t4.CreatedDate, t4.ChangedDate, NULL DeletedDate, NULL ParentSectionId, t4.Id CourseId, NULL Version, t1.title Title, t1.description Description, '0' IsDeleted,
	NULL Priority,t1.item_seq_path SectionSeqPath, t1.level Level, t3.ExtID CreatedBy, t3.ExtID ChangedBy, t4.Id ParentId, '0' ParentType,'0' [Order], t1.ID ExternalId
	FROM opa.Staging_CourseItems t1
	LEFT JOIN opa.Migration_Section t2 ON t1.Id = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.owner__id = t3.ID
	JOIN opa.Migration_Course t4 ON t1.course__id = t4.ExternalId 
	WHERE t2.ExternalId IS NULL
	AND t1.Type = 'folder' AND t1.partent__id <> 0
END