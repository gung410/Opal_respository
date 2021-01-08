
CREATE PROCEDURE opa.prc_CCPM_Staging_DevelopmentalRolesAndLearningFrameworkData_Insert
AS
BEGIN
	INSERT INTO opa.Staging_DevelopmentalRolesAndLearningFrameworkData (ID,code,category,description,parent_code,status)
	SELECT t1.ID,t1.code,t1.category,t1.description,t1.parent_code,t1.status
	FROM opa.Raw_DevelopmentalRolesAndLearningFrameworkData t1
	LEFT JOIN opa.Staging_DevelopmentalRolesAndLearningFrameworkData t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
