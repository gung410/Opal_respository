
CREATE PROCEDURE opa.prc_CCPM_Staging_Quiz_Insert
AS
BEGIN
	INSERT INTO opa.Staging_quiz(ID,name,title,description,type,relpath,level,owner__id,creator__id,group__id,seq,config,content_plate,extra_plate,qn_hints,ans_explanation,	
								 max_mark,is_auto_marked,show_feedback_after_answer,show_feedback,rand_qns,rand_choice,ctime)
	SELECT t1.ID,t1.name,t1.title,t1.description,t1.type,t1.relpath,t1.level,t1.owner__id,t1.creator__id,t1.group__id,t1.seq,t1.config,t1.content_plate,t1.extra_plate,t1.qn_hints,t1.ans_explanation,	
		   t1.max_mark,t1.is_auto_marked,t1.show_feedback_after_answer,t1.show_feedback,t1.rand_qns,t1.rand_choice,t1.ctime
	FROM opa.Raw_quiz t1
	LEFT JOIN opa.Staging_quiz t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
