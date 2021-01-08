IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_SharedQuestions_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_SharedQuestions_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_SharedQuestions_Insert
AS
BEGIN
	INSERT INTO opa.Migration_SharedQuestions (Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,Question_Type,Question_Title,Question_CorrectAnswer,Question_Options,
											   Question_Hint,Question_AnswerExplanatoryNote,Question_Level,OwnerId,IsDeleted)
	SELECT	NEWID() Id, CAST(t1.ctime AS DATETIME2(7)) CreatedDate, CAST(t1.ctime AS DATETIME2(7)) ChangedDate,  t1.id ExternalId, t4.ExtID CreatedBy, t4.ExtID ChangedBy,
			t1.type Question_Type,t1.description Question_Title,NULL Question_CorrectAnswer,NULL Question_Options,
			t1.qn_hints Question_Hint,t1.ans_explanation Question_AnswerExplanatoryNote, NULL Question_Level,t4.ExtID OwnerId, '0' IsDeleted
	FROM opa.Staging_Quiz t1
	LEFT JOIN opa.Migration_SharedQuestions t2 ON t1.Id = t2.ExternalId
	-- JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Staging_User t4 ON t1.owner__id = t4.ID
	WHERE t2.ExternalId IS NULL
	AND (t1.level <> 0 AND t1.level IS NOT NULL AND t1.level <> '')
END


