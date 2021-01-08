
CREATE PROCEDURE opa.prc_CCPM_Staging_Membership_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Membership (ID,UserID,GroupID,Role,ctime)
	SELECT t1.ID,t1.UserID,t1.GroupID,t1.Role,t1.ctime
	FROM opa.Raw_Membership t1
	LEFT JOIN opa.Staging_Membership t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
