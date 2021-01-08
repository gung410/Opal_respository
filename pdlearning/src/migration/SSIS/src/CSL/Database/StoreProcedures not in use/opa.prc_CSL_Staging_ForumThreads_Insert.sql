
CREATE PROCEDURE opa.prc_Humhub_Staging_ForumThreads_Insert
AS
BEGIN
	INSERT INTO opa.Staging_ForumThreads (id,forum__id,category__id,owner__id,title,description,status,last_post__id,ctime)
	SELECT t1.id,t1.forum__id,t1.category__id,t1.owner__id,t1.title,t1.description,t1.status,t1.last_post__id,t1.ctime
	FROM opa.Raw_ForumThreads t1
	LEFT JOIN opa.Staging_ForumThreads t2 ON t1.id = t2.id
	WHERE t2.id IS NULL
END
GO
