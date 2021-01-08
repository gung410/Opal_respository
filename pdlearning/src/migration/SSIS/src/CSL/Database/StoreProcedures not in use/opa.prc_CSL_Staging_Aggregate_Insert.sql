
ALTER PROCEDURE opa.prc_Humhub_Staging_Aggregate_Insert
AS
BEGIN
	INSERT INTO opa.Staging_Aggregate (id,label,type,about,keywords,intendedfor,parent__id,acs,status,creator__id,owner__id,ctime)
	SELECT t1.id,t1.label,t1.type,t1.about,t1.keywords,t1.intendedfor,t1.parent__id,t1.acs,t1.status,t1.creator__id,t1.owner__id,
	CAST(	CASE
			WHEN t1.ctime = ''
				THEN NULL
			ELSE t1.ctime
		END AS DATETIME) as ctime
	FROM opa.Raw_Aggregate t1
	LEFT JOIN opa.Staging_Aggregate t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
