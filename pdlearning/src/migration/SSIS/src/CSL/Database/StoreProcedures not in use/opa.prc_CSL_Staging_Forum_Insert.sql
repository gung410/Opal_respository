
CREATE PROCEDURE opa.prc_Humhub_Staging_Forum_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Forum (id,name,group_id,parent__id,owner__id,ctime)
	SELECT t1.id,t1.name,t1.group_id,t1.parent__id,t1.owner__id,t1.ctime
	FROM opa.Raw_Forum t1
	LEFT JOIN opa.Staging_Forum t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
