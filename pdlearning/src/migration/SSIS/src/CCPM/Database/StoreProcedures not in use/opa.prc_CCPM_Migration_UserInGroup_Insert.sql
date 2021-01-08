
CREATE PROCEDURE opa.prc_CCPM_Migration_UserInGroup_Insert
AS
BEGIN
	INSERT INTO opa.Migration_UserInGroup(Id,ExternalId,UserId,GroupId,UserRoleInGroup,CreatedDate,CreatedBy,ChangedDate,ChangedBy)
	SELECT NEWID(),t1.ID,t3.ID UserId,t4.ID GroupID,t1.Role,t1.ctime,NULL,GETDATE(),NULL
	FROM opa.Staging_Membership t1
	LEFT JOIN opa.Migration_UserInGroup t2 ON t1.ID = t2.ExternalId
	JOIN opa.Migration_User t3 ON t1.UserID = t3.ExternalId
	JOIN opa.Migration_Groups t4 ON t1.GroupID = t4.ExternalId
	WHERE t2.ExternalId IS NULL
END
