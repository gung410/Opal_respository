
CREATE PROCEDURE opa.prc_Humhub_Staging_Blog_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Blog (id,title,description,owner__id,group__id,ctime)
	SELECT t1.id,t1.title,t1.description,t1.owner__id,t1.group__id,
	CAST(	CASE
			WHEN t1.ctime = ''
				THEN NULL
			ELSE t1.ctime
		END AS DATETIME) as ctime
	FROM opa.Raw_Blog t1
	LEFT JOIN opa.Staging_Blog t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
