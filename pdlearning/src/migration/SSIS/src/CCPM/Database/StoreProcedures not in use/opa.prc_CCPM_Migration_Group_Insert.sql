
CREATE PROCEDURE opa.prc_CCPM_Migration_Group_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Groups (Id ,ExternalId, GroupName,GroupType,Status,CreatedDate,CreatedBy,ChangedDate,ChangedBy)
	SELECT NEWID(), t1.ID, t1.Label, t1.GroupType, t1.Status, t1.ctime,NULL,GETDATE(),NULL
	FROM opa.Staging_Groups t1
	LEFT JOIN opa.Migration_Groups t2 ON t1.ID = t2.ExternalId
	WHERE t2.ExternalId IS NULL
END
GO
