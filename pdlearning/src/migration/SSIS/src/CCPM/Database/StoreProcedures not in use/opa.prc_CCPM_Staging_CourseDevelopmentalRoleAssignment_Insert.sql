
CREATE PROCEDURE opa.prc_CCPM_Staging_CourseDevelopmentalRoleAssignment_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseDevelopmentalRoleAssignment (ID,course__id,course_code,code,source)
	SELECT t1.ID,t1.course__id,t1.course_code,t1.code,t1.source
	FROM opa.Raw_CourseDevelopmentalRoleAssignment t1
	LEFT JOIN opa.Staging_CourseDevelopmentalRoleAssignment t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
