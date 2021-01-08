
CREATE PROCEDURE opa.prc_CCPM_Staging_CourseItems_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseItems(ID,title,course__id,partent__id,type,seq,item_seq_path,level,relpath,description,duration,enabled_options,
									    enabled_stime,completion_req,owner__id,res__id,res_type)
	SELECT t1.ID,t1.title,t1.course__id,t1.partent__id,t1.type,t1.seq,t1.item_seq_path,t1.level,t1.relpath,t1.description,t1.duration,t1.enabled_options,
		   t1.enabled_stime,t1.completion_req,t1.owner__id,t1.res__id,t1.res_type
	FROM opa.Raw_CourseItems t1
	LEFT JOIN opa.Staging_CourseItems t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
