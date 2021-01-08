
CREATE PROCEDURE opa.prc_CCPM_Staging_CouresLearningFrameworksAssignment_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CouresLearningFrameworksAssignment(ID,course__id,course_code,lf_code,la_code,lsa_code,source)
	SELECT t1.ID,t1.course__id,t1.course_code,t1.lf_code,t1.la_code,t1.lsa_code,t1.source
	FROM opa.Raw_CouresLearningFrameworksAssignment t1
	LEFT JOIN opa.Staging_CouresLearningFrameworksAssignment t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
