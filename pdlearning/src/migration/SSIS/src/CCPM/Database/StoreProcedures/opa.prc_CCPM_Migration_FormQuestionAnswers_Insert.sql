IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_FormQuestionAnswers_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_FormQuestionAnswers_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_FormQuestionAnswers_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormQuestionAnswers(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,FormAnswerId,FormQuestionId,
												  AnswerValue, MaxScore, Score,ScoredBy,AnswerFeedback , SubmittedDate, SpentTimeInSeconds)
	SELECT NEWID() Id, t3.CreatedDate, t3.ChangedDate,t1.ID ExternalId, t3.CreatedBy,t3.CreatedBy ChangedBy, t3.Id FormAnswerId, t4.Id FormQuestionId,
	t1.response AnswerValue, t1.max_mark MaxScore, t1.score Score, NULL ScoredBy,NULL AnswerFeedback, NULL SubmittedDate, t1.total_duration SpentTimeInSeconds
	FROM opa.Staging_userquestionattempts t1
	LEFT JOIN opa.Migration_FormQuestionAnswers t2 ON t1.ID = t2.ExternalId
	JOIN opa.Migration_FormAnswers t3 ON t1.attempid = t3.ExternalId
	JOIN opa.Migration_FormQuestions t4 on t3.FormId = t4.FormId and t1.equizid = t4.ExternalId 
	WHERE t2.ExternalId IS NULL

END
