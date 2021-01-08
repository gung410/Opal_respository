IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_FormQuestions_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_FormQuestions_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_FormQuestions_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormQuestions (	Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,Question_Type,Question_Title,Question_CorrectAnswer,Question_Options,
												Question_Hint,Question_AnswerExplanatoryNote,Question_Level,FormId,Title,Priority,ShowFeedBackAfterAnswer,RandomizedOptions,Score,IsDeleted)
	SELECT	NEWID() Id, CAST(t1.ctime AS DATETIME2(7)) CreatedDate, CAST(t1.ctime AS DATETIME2(7)) ChangedDate,  t1.id ExternalId, t5.CreatedBy, t5.ChangedBy,
			t1.type Question_Type, t1.description Question_Title,NULL Question_CorrectAnswer, NULL Question_Options, t1.qn_hints Question_Hint, t1.ans_explanation Question_AnswerExplanatoryNote,
			NULL Question_Level,t5.Id FormId,NULL Title, 0 Priority,
			CASE WHEN t1.show_feedback_after_answer = 'yes' THEN 1
				 ELSE 0
			END AS  ShowFeedBackAfterAnswer, 
			t1.rand_choice RandomizedOptions, t1.max_mark Score, '0' IsDeleted
	FROM opa.Staging_Quiz t1
	LEFT JOIN opa.Migration_FormQuestions t2 ON t1.Id = t2.ExternalId
	JOIN opa.Migration_Forms t5 ON t5.ExternalId = LEFT(t1.relpath,CHARINDEX('/',t1.relpath)-1)
	WHERE t2.ExternalId IS NULL
	AND (t1.level <> 0 AND t1.level IS NOT NULL AND t1.level <> '')
END
