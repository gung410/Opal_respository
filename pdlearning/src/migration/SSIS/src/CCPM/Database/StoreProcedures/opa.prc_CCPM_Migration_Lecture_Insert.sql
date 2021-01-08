IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_Lecture_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_Lecture_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_Lecture_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Lecture (	Id,CreatedDate,ChangedDate,DeletedDate,SectionId,CourseId,Version,CopyRightId,LectureName,Description,
										Type,ThumbnailUrl,IsDeleted,Priority,CreatedBy,ChangedBy,ParentId,ParentType,[Order],LectureIcon,Status )


	SELECT	NEWID() Id, t4.CreatedDate, t4.ChangedDate, NULL DeletedDate, t4.Id SectionId, t4.CourseId CourseId, NULL Version, NULL CopyRightId, t1.title LectureName, t1.description Description, 
			NULL Type, NULL ThumbnailUrl, '0' IsDeleted, NULL Priority, t3.ExtID CreatedBy, t3.ExtID ChangedBy, t4.Id ParentId, '1' ParentType, '0' [Order], NULL LectureIcon, '0' Status 
	FROM opa.Staging_CourseItems t1
	LEFT JOIN opa.Migration_Lecture t2 ON t1.Id = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.owner__id = t3.ID
	JOIN opa.Migration_Section t4 on t1.partent__id = t4.ExternalId
	WHERE t2.ExternalId IS NULL
	AND t1.Type = 'resource'and partent__id <> 0
END