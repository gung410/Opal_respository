
CREATE PROCEDURE opa.prc_Humhub_Staging_WallPost_Insert
AS
BEGIN
	INSERT INTO opa.Staging_WallPost (id,group_id,title,isHL,content,type,url,res_slot,res_label,owner__id,ctime)
	SELECT t1.id,t1.group_id,t1.title,t1.isHL,t1.content,t1.type,t1.url,t1.res_slot,t1.res_label,t1.owner__id,
	CAST(	CASE
			WHEN t1.ctime = ''
				THEN NULL
			ELSE t1.ctime
		END AS DATETIME) as ctime
	FROM opa.Raw_WallPost t1
	LEFT JOIN opa.Staging_WallPost t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
