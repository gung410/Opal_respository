
CREATE PROCEDURE opa.prc_Humhub_Staging_BlogEntry_Insert
AS
BEGIN
	INSERT INTO opa.Staging_BlogEntry (id,blog__id,title,category,content,owner__id,stime,status,res_slot,res_file,ctime)
	SELECT t1.id,t1.blog__id,t1.title,t1.category,t1.content,t1.owner__id,t1.stime,t1.status,t1.res_slot,t1.res_file,
	CAST(	CASE
			WHEN t1.ctime = ''
				THEN NULL
			ELSE t1.ctime
		END AS DATETIME) as ctime
	FROM opa.Raw_BlogEntry t1
	LEFT JOIN opa.Staging_BlogEntry t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
