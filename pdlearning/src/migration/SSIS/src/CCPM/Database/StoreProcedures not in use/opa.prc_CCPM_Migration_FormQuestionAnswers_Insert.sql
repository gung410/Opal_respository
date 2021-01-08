ALTER PROCEDURE opa.prc_CCPM_Migration_FormQuestionAnswers_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormQuestionAnswers(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,FormAnswerId,FormQuestionId,
												  AnswerValue,Score,ScoredBy,AnswerFeedback)
	SELECT NEWID() Id, t1.last_attempted CreatedDate,NULL ChangedDate,t1.ID ExternalId, t3.CreatedBy,NULL ChangedBy, t3.Id FormAnswerId, t4.Id FormQuestionId,
	t1.response AnswerValue,t1.mark Score, NULL ScoredBy,NULL AnswerFeedback
	FROM opa.Staging_userquestionattempts t1
	LEFT JOIN opa.Migration_FormQuestionAnswers t2 ON t1.ID = t2.ExternalId
	JOIN opa.Migration_FormAnswers t3 ON t1.attempid = t3.ExternalId
	JOIN opa.Migration_FormQuestions t4 on t3.FormId = t4.FormId and t1.equizid = t4.ExternalId 
	WHERE t2.ExternalId IS NULL
END
GO
