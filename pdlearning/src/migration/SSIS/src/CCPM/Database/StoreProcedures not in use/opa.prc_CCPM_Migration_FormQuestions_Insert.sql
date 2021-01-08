
ALTER PROCEDURE opa.prc_CCPM_Migration_FormQuestions_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormQuestions(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,Question_Type,Question_Title,Question_CorrectAnswer,Question_Options,
											Question_Hint,Question_AnswerExplanatoryNote,Question_Level,FormId,Title,Priority,ShowFeedBackAfterAnswer,RandomizedOptions,Score,IsDeleted)

	SELECT NEWID() Id,t1.ctime CreatedDate,NULL ChangedDate,t1.ID ExternalId,t3.ExtID CreatedBy,NULL ChangedBy,t1.type Question_Type,t1.description Question_Title,
		   'NEED DEVELOPER INSERT' Question_CorrectAnswer,'NEED DEVELOPER INSERT' Question_Options,t1.qn_hints Question_Hint,t1.ans_explanation Question_AnswerExplanatoryNote,
		   t1.level Question_Level,t4.Id FormId,NULL Title,NULL Priority,t1.show_feedback_after_answer ShowFeedBackAfterAnswer,t1.rand_choice RandomizedOptions,t1.max_mark Score,0 IsDeleted
	FROM opa.Staging_quiz t1
	LEFT JOIN opa.Migration_FormQuestions t2 ON t1.ID = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Migration_Forms t4 ON t4.ExternalId = LEFT(t1.relpath,CHARINDEX('/',t1.relpath)-1)
	WHERE t2.ExternalId IS NULL
	AND (t1.level <> '')
END


