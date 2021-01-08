IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_PostFromWallPost_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_PostFromWallPost_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CSL_Migration_PostFromWallPost_Insert
AS
BEGIN

   SET NOCOUNT ON;
    declare @trancount int;
    set @trancount = @@trancount;
    begin try
        if @trancount = 0
            begin transaction
        else
            save transaction usp_my_procedure_name;

	DECLARE @PostID INT;
	SELECT @PostID = MAX(id) + 1 FROM opa.CSL_Migration_post;
	DECLARE @FileID INT;
	SELECT @FileID = ISNULL(MAX(id),0) + 1 FROM opa.CSL_Migration_file;
	IF @FileID = 1 
		BEGIN 
			SET @FileID = 3000
		END

	IF OBJECT_ID('tempdb.dbo.#temp_post', 'U') IS NOT NULL
	DROP TABLE #temp_post; 
	CREATE TABLE #temp_post(
	  id INT NOT NULL IDENTITY(3000,1),
	  message_2trash NVARCHAR(MAX),
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

	IF OBJECT_ID('tempdb.dbo.#temp_file', 'U') IS NOT NULL
	DROP TABLE #temp_file; 
	CREATE TABLE #temp_file(
	  id INT NOT NULL IDENTITY(1000,1),
	  guid varchar(45) DEFAULT NULL,
	  object_model varchar(100) DEFAULT '',
	  object_id varchar(100) DEFAULT '',
	  file_name nvarchar(255) DEFAULT NULL,
	  title nvarchar(255) DEFAULT NULL,
	  mime_type varchar(150) DEFAULT NULL,
	  size varchar(45)  DEFAULT NULL,
	  created_at datetime DEFAULT NULL,
	  created_by int DEFAULT NULL,
	  updated_at datetime DEFAULT NULL,
	  updated_by int DEFAULT NULL,
	  show_in_stream tinyint DEFAULT '1',
	  external_id VARCHAR(255) DEFAULT NULL,
	  location VARCHAR(255) DEFAULT NULL,
	  PRIMARY KEY (id)
	)

	DBCC checkident(#temp_file, reseed, @FileID)

	INSERT INTO #temp_post(message_2trash,message,created_at,created_by,updated_at,updated_by,external_id)
	SELECT t1.filename message_2trash,  
		CASE 
			WHEN t1.title IS NOT NULL OR t1.title <> '' THEN CONCAT('# ',[dbo].[ConvertHTML](ISNULL(t1.title,'')),CHAR(13),CHAR(13),[dbo].[ConvertHTML](ISNULL(t1.content,'')))
			ELSE [dbo].[ConvertHTML](ISNULL(t1.content,'')) 
		END AS message,
	t1.ctime created_at, t2.Opal2ID created_by,t1.ctime updated_at, t2.Opal2ID updated_by,t1.id
	FROM opa.Staging_WallPost t1 (NOLOCK)
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id
	/*
	UPDATE #temp_post
	SET message =  CONCAT(message,'
	**Reference**: ', message_2trash)
	WHERE message_2trash IS NOT NULL AND message_2trash <> ''
	*/
	SET IDENTITY_INSERT opa.CSL_Migration_post ON 

	INSERT INTO opa.CSL_Migration_post(id,message_2trash,message,created_at,created_by,updated_at,updated_by)
	SELECT t1.id,t1.message_2trash,t1.message,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by
	FROM #temp_post t1 
	LEFT JOIN opa.CSL_Migration_post t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_post OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '5. Wall Posts' source_file,'opa.Staging_WallPost' source_table,t1.external_id source_id, 'post' destination_table, t1.id destination_id
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
		   t1.updated_at,t1.updated_by, t3.destination_id contentcontainer_id, t1.created_at stream_sort_date,'default' stream_channel
	FROM #temp_post t1
	JOIN opa.Staging_WallPost t2 ON t1.external_id = t2.id
	JOIN mig.CSL_ObjectMapping t3 ON t2.group_id = t3.source_id AND t3.source_table = 'opa.Staging_Aggregate' AND t3.destination_table = 'contentcontainer'
	LEFT JOIN opa.CSL_Migration_content t4 ON t1.id = t4.object_id AND t4.object_model = 'humhub\modules\post\models\Post' AND t4.stream_channel = 'default'
	WHERE t4.object_id IS NULL


	INSERT INTO opa.CSL_Migration_content_tag_relation(content_id,tag_id)
	SELECT t1.id content_id,t2.id tag_id
	FROM opa.CSL_Migration_content t1 (NOLOCK)
	JOIN opa.CSL_Migration_content_tag t2 ON t1.object_model = 'humhub\modules\post\models\Post' AND t1.contentcontainer_id = t2.contentcontainer_id
	LEFT JOIN opa.CSL_Migration_content_tag_relation t4 ON t1.id = t4.content_id 
	WHERE t2.module_id = 'topic' 
	AND t2.name = 'Wall Post'
	AND t4.content_id IS NULL

	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\activity\models\Activity' object_model, t1.id object_id,t2.visibility, t2.pinned, t2.archived, t2.created_at, t2.created_by,
		   t2.updated_at,t2.updated_by, t2.contentcontainer_id, t2.stream_sort_date, 'activity' stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN opa.CSL_Migration_content t2 ON t1.object_id = t2.object_id AND t2.stream_channel = 'default' AND t2.object_model = 'humhub\modules\post\models\Post'
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.object_id = t3.object_id AND t3.object_model = 'humhub\modules\activity\models\Activity' and t3.stream_channel  = 'activity'
	WHERE  t3.object_id IS NULL

	INSERT INTO opa.CSL_Migration_user_follow ( object_model,object_id,user_id,send_notifications)
	SELECT 'humhub\modules\post\models\Post', t1.id, t1.created_by, '1'
	FROM #temp_post t1 
	LEFT JOIN opa.CSL_Migration_user_follow t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\post\models\Post' AND t1.created_by = t2.user_id
	WHERE t2.object_id IS NULL

	/***** insert file start *****/

	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\post\models\Post' object_model, t1.id object_id, 
			ISNULL(res_label,RIGHT(t2.res_slot, CHARINDEX('/',REVERSE(t2.res_slot))-1)) file_name,
			t2.title title,
			RIGHT(t2.res_slot, CHARINDEX('.',REVERSE(t2.res_slot))-1) mime_type,
			'0' size, t1.created_at,t1.created_by,t1.updated_at,t1.updated_by,'1' show_in_stream, t2.id external_id, t2.res_slot location
	FROM #temp_post t1 (NOLOCK)
	JOIN opa.Staging_WallPost t2 ON t1.external_id = t2.id
	WHERE t2.res_slot IS NOT NULL AND t2.res_slot <> ''

	SET IDENTITY_INSERT opa.CSL_Migration_file ON

	INSERT INTO opa.CSL_Migration_file(id,guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,location)
	SELECT t1.id,t1.guid,t1.object_model,t1.object_id,t1.file_name,t1.title,t1.mime_type,t1.size,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by,t1.show_in_stream,t1.location
	FROM #temp_file t1
	LEFT JOIN opa.CSL_Migration_file t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_file OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '5. Wall Posts' source_file,'opa.Staging_WallPost' source_table,t1.external_id source_id, 'file' destination_table, t1.id destination_id
	FROM #temp_file t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'file'
	WHERE t2.destination_id IS NULL


 if @trancount = 0
            commit;
    end try
    begin catch
        declare @error int, @message varchar(4000), @xstate int;
        select @error = ERROR_NUMBER(), @message = ERROR_MESSAGE(), @xstate = XACT_STATE();
        if @xstate = -1
            rollback;
        if @xstate = 1 and @trancount = 0
            rollback
        if @xstate = 1 and @trancount > 0
            rollback transaction usp_my_procedure_name;

        raiserror ('opa.prc_CSL_Migration_PostFromWallPost_Insert: %d: %s', 16, 1, @error, @message) ;
    end catch

END