
CREATE PROCEDURE opa.prc_CCPM_Staging_Group_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Groups (ID,Label,GroupType,	Status,ctime)
	SELECT t1.ID, t1.Label, t1.GroupType, t1.Status, t1.ctime
	FROM opa.Raw_Groups t1
	LEFT JOIN opa.Staging_Groups t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
