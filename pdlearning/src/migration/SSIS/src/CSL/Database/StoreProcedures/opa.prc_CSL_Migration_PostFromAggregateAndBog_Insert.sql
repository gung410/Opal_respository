IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_PostFromAggregateAndBog_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_PostFromAggregateAndBog_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CSL_Migration_PostFromAggregateAndBog_Insert
AS
BEGIN

BEGIN TRANSACTION 

	IF OBJECT_ID('tempdb.dbo.#temp_post', 'U') IS NOT NULL
	DROP TABLE #temp_post; 
	CREATE TABLE #temp_post(
	  id INT NOT NULL IDENTITY(1000,1),
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
	
	INSERT INTO #temp_post(message_2trash,message,created_at,created_by,updated_at,updated_by,external_id)
	SELECT NULL message_2trash, 
		CASE 
			WHEN (t1.about IS NOT NULL) AND (t2.title IS NOT NULL)  
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.about,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t2.title,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t2.description,'')))
			WHEN (t1.about IS NOT NULL) AND (t2.title IS NULL)
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.about,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t2.description,'')))
			WHEN (t1.about IS NULL ) AND (t2.title IS NOT NULL)
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t2.title,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t2.description,'')))
			ELSE [dbo].[ConvertHTML](ISNULL(t2.description,''))
		END AS message,
	t2.ctime created_at, t3.Opal2ID created_by,t2.ctime updated_at, t3.Opal2ID updated_by,t2.id
	FROM opa.Staging_Aggregate t1
	JOIN opa.Staging_Blog t2 ON t1.id = t2.group__id
	JOIN opa.Staging_User t3 ON t2.owner__id = t3.Id
	WHERE (t2.description IS NOT NULL)  

	INSERT INTO #temp_post(message_2trash,message,created_at,created_by,updated_at,updated_by,external_id)
	SELECT NULL message_2trash, 
		CASE 
			WHEN (t1.about IS NOT NULL) AND (t2.title IS NOT NULL)  
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.about,'')), CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t2.title,'')))
			WHEN (t1.about IS NOT NULL) AND (t2.title IS NULL)
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.about,'')))
			WHEN (t1.about IS NULL ) AND (t2.title IS NOT NULL)
				THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t2.title,'')))
			ELSE ''
		END AS message,
	t2.ctime created_at, t3.Opal2ID created_by,t2.ctime updated_at, t3.Opal2ID updated_by,t2.id
	FROM opa.Staging_Aggregate t1
	JOIN opa.Staging_Blog t2 ON t1.id = t2.group__id
	JOIN opa.Staging_User t3 ON t2.owner__id = t3.Id
	WHERE (t2.description IS NULL)  

	SET IDENTITY_INSERT opa.CSL_Migration_post ON 

	INSERT INTO opa.CSL_Migration_post(id,message_2trash,message,created_at,created_by,updated_at,updated_by)
	SELECT t1.id,t1.message_2trash,t1.message,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by
	FROM #temp_post t1 
	LEFT JOIN opa.CSL_Migration_post t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_post OFF 

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '7. Blog' source_file,'opa.Staging_Blog' source_table,t1.external_id source_id, 'post' destination_table, t1.id destination_id
	FROM #temp_post t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'post' AND t2.source_table = 'opa.Staging_Blog'
	WHERE t2.destination_id IS NULL

	INSERT INTO opa.CSL_Migration_activity(class,module,object_model,object_id)
	SELECT 'humhub\modules\content\activities\ContentCreated' class, 'content' module, 'humhub\modules\post\models\Post' object_model, t1.id
	FROM #temp_post t1
	LEFT JOIN opa.CSL_Migration_activity t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\post\models\Post' AND t2.class = 'humhub\modules\content\activities\ContentCreated'
	WHERE t2.object_id IS NULL

	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\post\models\Post' object_model, t1.id object_id, '0' visibility, '1' pinned, '0' archived, t1.created_at, t1.created_by,
		   t1.updated_at,t1.updated_by, t3.destination_id contentcontainer_id, t1.created_at stream_sort_date,'default' stream_channel
	FROM #temp_post t1
	JOIN opa.Staging_Blog t2 ON t1.external_id = t2.id
	JOIN mig.CSL_ObjectMapping t3 ON t2.group__id = t3.source_id AND t3.source_table = 'opa.Staging_Aggregate' AND t3.destination_table = 'contentcontainer'
	LEFT JOIN opa.CSL_Migration_content t4 ON t1.id = t4.object_id AND t4.object_model = 'humhub\modules\post\models\Post' AND t4.stream_channel = 'default'
	WHERE t4.object_id IS NULL

	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\activity\models\Activity' object_model, t1.id object_id,t2.visibility, t2.pinned, t2.archived, t2.created_at, t2.created_by,
		   t2.updated_at,t2.updated_by, t2.contentcontainer_id, t2.stream_sort_date, 'activity' stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN opa.CSL_Migration_content t2 ON t1.object_id = t2.object_id AND t2.stream_channel = 'default' AND t2.object_model = 'humhub\modules\post\models\Post'
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.id = t3.object_id AND t3.object_model = 'humhub\modules\activity\models\Activity' AND t3.stream_channel = 'activity'
	WHERE t3.object_id IS NULL

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