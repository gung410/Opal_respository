
CREATE PROCEDURE opa.prc_CCPM_Staging_CourseItemAttempts_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseItemAttempts(ID,user__id,course__id,item__id,seq,completion_status,progress_measure,score,stime,etime)
	SELECT t1.ID,t1.user__id,t1.course__id,t1.item__id,t1.seq,t1.completion_status,t1.progress_measure,t1.score,t1.stime,t1.etime
	FROM opa.Raw_CourseItemAttempts t1
	LEFT JOIN opa.Staging_CourseItemAttempts t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
