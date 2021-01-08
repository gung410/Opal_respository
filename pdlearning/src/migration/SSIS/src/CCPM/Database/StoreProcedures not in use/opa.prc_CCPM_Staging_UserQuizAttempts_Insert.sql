
CREATE PROCEDURE opa.prc_CCPM_Staging_UserQuizAttempts_Insert
AS
BEGIN
	INSERT INTO opa.Staging_userquizattempts(ID,user__id,equiz__id,mark,max_mark,score,progress,duration,attempt_time,attempt_complete,rand_qns,
				all_qns_count,select_qns_count,qns_order,all_qns_order,all_qns_level_type,seq)
	SELECT t1.ID,t1.user__id,t1.equiz__id,t1.mark,t1.max_mark,t1.score,t1.progress,t1.duration,t1.attempt_time,t1.attempt_complete,t1.rand_qns,
		   t1.all_qns_count,t1.select_qns_count,t1.qns_order,t1.all_qns_order,t1.all_qns_level_type,t1.seq
	FROM opa.Raw_userquizattempts t1
	LEFT JOIN opa.Staging_userquizattempts t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
