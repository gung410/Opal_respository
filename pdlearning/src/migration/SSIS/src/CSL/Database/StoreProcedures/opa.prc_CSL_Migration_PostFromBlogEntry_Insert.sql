IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_PostFromBlogEntry_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_PostFromBlogEntry_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CSL_Migration_PostFromBlogEntry_Insert
AS
BEGIN

BEGIN TRANSACTION 

	DECLARE @PostID INT;
	SELECT @PostID = MAX(id) + 1 FROM opa.CSL_Migration_post;

	IF OBJECT_ID('tempdb.dbo.#temp_post', 'U') IS NOT NULL
	DROP TABLE #temp_post; 
	CREATE TABLE #temp_post(
	  id INT NOT NULL IDENTITY(2000,1),
	  message_2trash TEXT,
	  message NTEXT,
	  url VARCHAR(255) DEFAULT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  external_id VARCHAR(255) DEFAULT NULL,
	PRIMARY KEY (id)
	)

	DBCC checkident(#temp_post, reseed, @PostID)

	IF OBJECT_ID('tempdb.dbo.#temp_comment', 'U') IS NOT NULL
	DROP TABLE #temp_comment; 
	CREATE TABLE #temp_comment(
	  id INT NOT NULL IDENTITY(1000,1),
	  message NTEXT,
	  object_model VARCHAR(100) NOT NULL,
	  object_id INT NOT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  external_id VARCHAR(255) DEFAULT NULL,
	PRIMARY KEY (id)
	)


	INSERT INTO #temp_post(message_2trash,message,created_at,created_by,updated_at,updated_by,external_id)
	SELECT NULL message_2trash,
		CASE 
			WHEN t1.title IS NOT NULL AND t1.title <> '' THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.title,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t1.content,'')))
			ELSE [dbo].[ConvertHTML](ISNULL(t1.content,'')) 
		END AS message,
	t1.ctime created_at, t2.Opal2ID created_by,t2.ctime updated_at, t2.Opal2ID updated_by,t1.id
	FROM opa.Staging_BlogEntry t1
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id

	SET IDENTITY_INSERT opa.CSL_Migration_post ON 

	INSERT INTO opa.CSL_Migration_post(id,message_2trash,message,created_at,created_by,updated_at,updated_by)
	SELECT t1.id,t1.message_2trash,t1.message,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by
	FROM #temp_post t1 
	LEFT JOIN opa.CSL_Migration_post t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_post OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '7. Blog' source_file,'opa.Staging_BlogEntry' source_table,t1.external_id source_id, 'post' destination_table, t1.id destination_id
	FROM #temp_post t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'post'
	WHERE t2.destination_id IS NULL

	INSERT INTO opa.CSL_Migration_activity(class,module,object_model,object_id)
	SELECT 'humhub\modules\content\activities\ContentCreated' class, 'content' module, 'humhub\modules\post\models\Post' object_model, t1.id
	FROM #temp_post t1
	LEFT JOIN opa.CSL_Migration_activity t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\post\models\Post' AND t2.class = 'humhub\modules\content\activities\ContentCreated'
	WHERE t2.object_id IS NULL

	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\post\models\Post' object_model, t1.id object_id, '0' visibility, '0' pinned, '0' archived, t1.created_at, t1.created_by,
		   t1.updated_at,t1.updated_by, t4.destination_id contentcontainer_id, t1.created_at stream_sort_date,'default' stream_channel
	FROM #temp_post t1
	JOIN opa.Staging_BlogEntry t2 ON t1.external_id = t2.id
	JOIN opa.Staging_Blog t3 ON t2.blog__id = t3.id
	JOIN mig.CSL_ObjectMapping t4 ON t3.group__id = t4.source_id AND t4.source_table = 'opa.Staging_Aggregate' AND t4.destination_table = 'contentcontainer'
	LEFT JOIN opa.CSL_Migration_content t5 ON t1.id = t5.object_id AND t5.object_model = 'humhub\modules\post\models\Post' AND t5.stream_channel = 'default'
	WHERE t5.object_id IS NULL

	INSERT INTO opa.CSL_Migration_content_tag_relation(content_id,tag_id)
	SELECT t1.id content_id,t2.id tag_id
	FROM opa.CSL_Migration_content t1
	JOIN opa.CSL_Migration_content_tag t2 ON t1.object_model = 'humhub\modules\post\models\Post' AND t1.contentcontainer_id = t2.contentcontainer_id
	LEFT JOIN opa.CSL_Migration_content_tag_relation t4 ON t1.id = t4.content_id
	WHERE t2.module_id = 'topic' 
	AND t2.name = 'Blog'
	AND t4.content_id IS NULL



	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\activity\models\Activity' object_model, t1.id object_id,t2.visibility, t2.pinned, t2.archived, t2.created_at, t2.created_by,
		   t2.updated_at,t2.updated_by, t2.contentcontainer_id, t2.stream_sort_date, 'activity' stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN opa.CSL_Migration_content t2 ON t1.object_id = t2.object_id AND t2.stream_channel = 'default' AND t2.object_model = 'humhub\modules\post\models\Post'
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.object_id = t3.object_id AND t3.object_model = 'humhub\modules\activity\models\Activity' and t3.stream_channel  = 'activity'
	WHERE  t3.object_id IS NULL

	/***** insert blog_comment *****/

	INSERT INTO #temp_comment(message,object_model,object_id,created_at,created_by,updated_at,updated_by,external_id)
	SELECT [dbo].[ConvertHTML](ISNULL(t1.comment,'')), 'humhub\modules\post\models\Post' object_model, t2.id object_id, t1.ctime created_at, t3.Opal2ID created_by, t1.ctime updated_at, t3.Opal2ID updated_by, t1.id
	FROM opa.Staging_BlogComments t1
	JOIN #temp_post t2 ON t1.entry__id = t2.external_id
	JOIN opa.Staging_User t3 ON t1.owner__id = t3.Id


	SET IDENTITY_INSERT opa.CSL_Migration_comment ON

	INSERT INTO opa.CSL_Migration_comment(id,message,object_model,object_id,created_at,created_by,updated_at,updated_by)
	SELECT t1.id,t1.message,t1.object_model,t1.object_id,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by
	FROM #temp_comment t1
	LEFT JOIN opa.CSL_Migration_comment t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_comment OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '7. Blog' source_file,'opa.Staging_BlogComment' source_table,t1.external_id source_id, 'comment' destination_table, t1.id destination_id
	FROM #temp_comment t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'comment'
	WHERE t2.destination_id IS NULL

	INSERT INTO opa.CSL_Migration_activity(class,module,object_model,object_id)
	SELECT 'humhub\modules\comment\activities\NewComment' class, 'comment' module, 'humhub\modules\comment\models\Comment' object_model, t1.id object_id
	FROM #temp_comment t1
	LEFT JOIN opa.CSL_Migration_activity t2 ON t1.id = t2.object_id AND t2.class = 'humhub\modules\comment\activities\NewComment' AND t2.module  = 'comment' AND t2.object_model = 'humhub\modules\comment\models\Comment'
	WHERE  t2.object_id IS NULL

	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\activity\models\Activity' object_model, t1.id object_id, '0' visibility, '0' pinned, '0' archived, t2.created_at, t2.created_by,
		   t2.updated_at,t2.updated_by, t6.destination_id contentcontainer_id, t2.created_at stream_sort_date, 'activity' stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN #temp_comment t2 ON t1.object_id = t2.id
	JOIN #temp_post t3 ON t2.object_id = t3.id
	JOIN opa.Staging_BlogEntry t4 ON t3.external_id = t4.id
	JOIN opa.Staging_Blog t5 ON t4.blog__id = t5.id
	JOIN mig.CSL_ObjectMapping t6 ON t5.group__id = t6.source_id AND t6.source_table = 'opa.Staging_Aggregate' AND t6.destination_table = 'contentcontainer'
	LEFT JOIN opa.CSL_Migration_content t7 ON t1.id = t7.object_id AND t7.object_model = 'humhub\modules\activity\models\Activity' and t7.stream_channel  = 'activity'
	WHERE  t7.object_id IS NULL


	INSERT INTO opa.CSL_Migration_user_follow ( object_model,object_id,user_id,send_notifications)
	SELECT 'humhub\modules\post\models\Post', t1.id, t1.created_by, '1'
	FROM #temp_post t1 
	LEFT JOIN opa.CSL_Migration_user_follow t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\post\models\Post' AND t1.created_by = t2.user_id
	WHERE t2.object_id IS NULL


IF (@@Error <> 0) 
   BEGIN          
        ROLLBACK TRANSACTION       
   END 
   ELSE 
        COMMIT TRANSACTION

END