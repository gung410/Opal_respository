
CREATE PROCEDURE opa.prc_CCPM_Staging_CourseMaterials_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseMaterials(ID,title,course__id,type,equiz__id,equiz_name,url,page_content,scorm_name,attach_slot,attach_filename,
											attach_size,width,height,description,duration,owner__id,ctime)
	SELECT t1.ID,t1.title,t1.course__id,t1.type,t1.equiz__id,t1.equiz_name,t1.url,t1.page_content,t1.scorm_name,t1.attach_slot,t1.attach_filename,
		   t1.attach_size,t1.width,t1.height,t1.description,t1.duration,t1.owner__id,t1.ctime
	FROM opa.Raw_CourseMaterials t1
	LEFT JOIN opa.Staging_CourseMaterials t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
