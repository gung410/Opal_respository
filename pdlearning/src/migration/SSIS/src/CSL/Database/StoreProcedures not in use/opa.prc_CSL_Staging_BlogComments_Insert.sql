
CREATE PROCEDURE opa.prc_Humhub_Staging_BlogComments_Insert
AS
BEGIN
	INSERT INTO opa.Staging_BlogComments (id,entry__id,comment,owner__id,ctime)
	SELECT t1.id,t1.entry__id,t1.comment,t1.owner__id,
	CAST(	CASE
			WHEN t1.ctime = ''
				THEN NULL
			ELSE t1.ctime
		END AS DATETIME) as ctime
	FROM opa.Raw_BlogComments t1
	LEFT JOIN opa.Staging_BlogComments t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
