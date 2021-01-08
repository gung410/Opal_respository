
ALTER PROCEDURE opa.prc_CCPM_Migration_SharedQuestions_Insert
AS
BEGIN
	INSERT INTO opa.Migration_SharedQuestions(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,Question_Type,Question_Title,Question_CorrectAnswer,
											  Question_Options,Question_Hint,Question_AnswerExplanatoryNote,Question_Level,OwnerId,IsDeleted)

	SELECT NEWID() Id,t1.ctime CreatedDate, NULL ChangedDate,t1.ID ExternalId,t3.ExtID CreatedBy,NULL ChangedBy,t1.type Question_Type,t1.description Question_Title,
		   'NEED DEVELOPER INSERT' Question_CorrectAnswer,'NEED DEVELOPER INSERT' Question_Options,t1.qn_hints Question_Hint,t1.ans_explanation Question_AnswerExplanatoryNote,
		   t1.level Question_Level,t4.ExtID OwnerId,0 IsDeleted
	FROM opa.Staging_quiz t1
	LEFT JOIN opa.Migration_SharedQuestions t2 ON t1.ID = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Staging_User t4 ON t1.owner__id = t4.ID
	WHERE t2.ExternalId IS NULL
	AND (t1.level <> '')
END
