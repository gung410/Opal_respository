
CREATE PROCEDURE opa.prc_CCPM_Staging_CourseDetail_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseDetail (ID,code,course_code,class_code,coursetitle,description,objective,targetaudience,sdate,edate,duration_hour,duration_minutes,end_of_reg,
										  traisi_is_listed,traisi_course_type,eduLevelPri,eduLevelSec,eduLevelPreU,keywords,trainingagency,agg_Id,creator__id,owner__id,
										  source,status,is_approve,ctime,publisher__id,publish_time)
	SELECT t1.ID,t1.code,t1.course_code,t1.class_code,t1.coursetitle,t1.description,t1.objective,t1.targetaudience,t1.sdate,t1.edate,t1.duration_hour,t1.duration_minutes,t1.end_of_reg,
		   t1.traisi_is_listed,t1.traisi_course_type,t1.eduLevelPri,t1.eduLevelSec,t1.eduLevelPreU,t1.keywords,t1.trainingagency,t1.agg_Id,t1.creator__id,t1.owner__id,
		   t1.source,t1.status,t1.is_approve,t1.ctime,t1.publisher__id,t1.publish_time
	FROM opa.Raw_CourseDetail t1
	LEFT JOIN opa.Staging_CourseDetail t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
