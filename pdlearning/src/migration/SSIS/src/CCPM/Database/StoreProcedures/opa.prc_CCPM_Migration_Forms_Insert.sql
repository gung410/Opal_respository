IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_Forms_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_Forms_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_Forms_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Forms (Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,LectureId,CourseId,SectionId,Title,Type,Status,
								OwnerId,IsDeleted,DueDate,InSecondTimeLimit,RandomizedQuestions,MaxAttempt,ShowQuizSummary)
	SELECT NEWID() Id, CAST(t1.ctime AS DATETIME2(7)) CreatedDate, CAST(t1.ctime AS DATETIME2(7)) ChangedDate, t1.id ExternalId, t3.ExtID CreatedBy, t3.ExtID ChangedBy,
		   NULL LectureId, NULL CourseId,NULL SectionId, t1.title, t1.type, 'Draft' Status,
		   t4.ExtID OwnerId, '0' IsDeleted, NULL DueDate, NULL InSecondTimeLimit, t1.rand_qns RandomizedQuestions, NULL MaxAttempt, 
		   CASE WHEN t1.show_feedback = '1' THEN 1
				ELSE 0
		   END AS ShowQuizSummary
	FROM opa.Staging_Quiz t1
	LEFT JOIN opa.Migration_Forms t2 ON t1.Id = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Staging_User t4 ON t1.owner__id = t4.ID
	WHERE t2.ExternalId IS NULL 
	AND (t1.level = 0 OR t1.level IS NULL OR t1.level = '')
END
