
CREATE PROCEDURE opa.prc_CCPM_Staging_UserQuestionAttempts_Insert
AS
BEGIN
	INSERT INTO opa.Staging_userquestionattempts(ID,equizid,attempid,response,is_correct,mark,max_mark,ori_mark,score,progress,total_duration,last_attempted)
	SELECT t1.ID,t1.equizid,t1.attempid,t1.response,t1.is_correct,t1.mark,t1.max_mark,t1.ori_mark,t1.score,t1.progress,t1.total_duration,t1.last_attempted
	FROM opa.Raw_userquestionattempts t1
	LEFT JOIN opa.Staging_userquestionattempts t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
	AND t1.is_correct <> 'unknown'
END
GO
