IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_Forum_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_Forum_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CSL_Migration_Forum_Insert
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

	DECLARE @CommentID INT;
	DECLARE @FileID INT;
	SELECT @FileID = ISNULL(MAX(id),0) + 1 FROM opa.CSL_Migration_file;

	SELECT @CommentID = ISNULL(MAX(id),0) + 1 FROM opa.CSL_Migration_comment;


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

	DBCC checkident(#temp_comment, reseed, @CommentID)


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

	IF OBJECT_ID('tempdb.dbo.#temp_forum_thread', 'U') IS NOT NULL
	DROP TABLE #temp_forum_thread; 
	CREATE TABLE #temp_forum_thread(
	  id int NOT NULL IDENTITY(3000,1),
	  title nvarchar(1000) NOT NULL,
	  is_home tinyint NOT NULL DEFAULT '0',
	  admin_only tinyint NOT NULL DEFAULT '0',
	  is_category tinyint DEFAULT '0',
	  parent_thread_id int DEFAULT NULL,
	  sort_order int DEFAULT '0',
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  external_id VARCHAR(255) DEFAULT NULL,
	  group_id INT DEFAULT NULL,
	PRIMARY KEY (id)
	)

	IF OBJECT_ID('tempdb.dbo.#temp_forum_thread_revision', 'U') IS NOT NULL
	DROP TABLE #temp_forum_thread_revision; 
	CREATE TABLE #temp_forum_thread_revision(
	  id int NOT NULL IDENTITY(4000,1),
	  revision bigint NOT NULL,
	  is_latest tinyint NOT NULL DEFAULT '0',
	  forum_thread_id int NOT NULL,
	  user_id int NOT NULL,
	  content ntext,
	  external_id VARCHAR(255) DEFAULT NULL,
	  is_comment tinyint NOT NULL DEFAULT '0',
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	PRIMARY KEY (id)
	)

	IF OBJECT_ID('tempdb.dbo.#temp_forum_parent', 'U') IS NOT NULL
	DROP TABLE #temp_forum_parent; 
	CREATE TABLE #temp_forum_parent(
	  id int NOT NULL,
	  external_id VARCHAR(255) DEFAULT NULL,
	PRIMARY KEY (id)
	)

	/***** insert forum_thread start *****/

	INSERT INTO #temp_forum_thread(title,is_home, admin_only,is_category,parent_thread_id, sort_order,created_at, created_by, updated_at, updated_by, external_id, group_id)
	SELECT [dbo].[ConvertHTML](ISNULL(t1.name,'')) title, '0' is_home, '0' admin_only,
	CASE 
		WHEN t1.parent__id <> 0 THEN 0
		ELSE 1
	END AS is_category,
	CASE 
		WHEN t1.parent__id <> 0 THEN t1.parent__id
		ELSE NULL
	END AS parent_thread_id,
	 '0'sort_order,
	t1.ctime created_at , t3.Opal2ID created_by,t1.ctime updated_at, t3.Opal2ID updated_by,t1.id, t1.group_id
	FROM opa.Staging_Forum t1 (NOLOCK)
	JOIN opa.Staging_Aggregate t2 ON t1.group_id = t2.id
	JOIN opa.Staging_User t3 ON t2.creator__id = t3.Id
	WHERE t1.owner__id = 0

	INSERT INTO #temp_forum_thread(title,is_home, admin_only,is_category,parent_thread_id, sort_order,created_at, created_by, updated_at, updated_by, external_id, group_id)
	SELECT [dbo].[ConvertHTML](ISNULL(t1.name,'')) title, '0' is_home, '0' admin_only,
	CASE 
		WHEN t1.parent__id <> 0 THEN 0
		ELSE 1
	END AS is_category,
		CASE 
		WHEN t1.parent__id <> 0 THEN t1.parent__id
		ELSE NULL
	END AS parent_thread_id, '0'sort_order,
	t1.ctime created_at , t2.Opal2ID created_by,t1.ctime updated_at, t2.Opal2ID updated_by,t1.id, t1.group_id
	FROM opa.Staging_Forum t1 (NOLOCK)
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id
	WHERE t1.owner__id <> 0

	INSERT INTO #temp_forum_thread(title,is_home, admin_only,is_category,parent_thread_id, sort_order,created_at, created_by, updated_at, updated_by, external_id, group_id)
	SELECT [dbo].[ConvertHTML](ISNULL(t1.title,'')) title, '0' is_home, '0' admin_only, '0' is_category, t1.forum__id parent_thread_id, '0'sort_order,
	t1.ctime created_at, t2.Opal2ID created_by,t1.ctime updated_at, t2.Opal2ID updated_by,t1.id, t3.group_id
	FROM opa.Staging_ForumThread t1 (NOLOCK)
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id
	JOIN opa.Staging_Forum t3 ON t1.forum__id =  t3.id

	INSERT INTO #temp_forum_parent(id, external_id)
	SELECT DISTINCT id, external_id
	FROM #temp_forum_thread

	UPDATE #temp_forum_thread 
	SET parent_thread_id = t2.id
	FROM #temp_forum_thread t1
	JOIN #temp_forum_parent t2 ON t1.parent_thread_id = t2.external_id








	SET IDENTITY_INSERT opa.CSL_Migration_forum_thread ON 

	INSERT INTO opa.CSL_Migration_forum_thread(id,title,is_home, admin_only,is_category,parent_thread_id, sort_order)
	SELECT t1.id,t1.title,t1.is_home, t1.admin_only,t1.is_category,t1.parent_thread_id, t1.sort_order
	FROM #temp_forum_thread t1
	LEFT JOIN opa.CSL_Migration_forum_thread t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_forum_thread OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '6. Forum' source_file,
		CASE 
			WHEN t1.is_category = 1 THEN 'opa.Staging_Forum'
			ELSE 'opa.Staging_ForumThread'
		END AS source_table,
		t1.external_id source_id, 'forum_thread' destination_table, t1.id destination_id
	FROM #temp_forum_thread t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'forum_thread'
	WHERE t2.destination_id IS NULL

	/***** insert forum_thread_revision start *****/

	INSERT INTO #temp_forum_thread_revision( revision, is_latest, forum_thread_id, user_id, content, external_id, is_comment,created_at, created_by, updated_at, updated_by)
	SELECT CONCAT(CONVERT(INT, CONVERT(DATETIME,GETDATE())),  CONVERT(INT, REPLACE( CONVERT(VARCHAR(8), GETDATE(), 108),':', '')) ) revision,
		CASE
			WHEN t1.id = t1.first_post__id THEN 1
			ELSE 0
		END AS is_latest,
		t3.id forum_thread_id, t2.Opal2ID user_id,[dbo].[ConvertHTML](ISNULL(t1.content,'')), t1.id,
		CASE
			WHEN t1.id = t1.parent__id THEN 0
			ELSE 1
		END AS is_comment,
		t1.ctime created_at, t2.Opal2ID created_by, 
		t1.last_edit_time updated_at, t4.Opal2ID updated_by
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id
	LEFT JOIN #temp_forum_parent t3 ON t1.thread__id = t3.external_id
	JOIN opa.Staging_User t4 ON t1.last_edit_uid = t4.Id


	SET IDENTITY_INSERT opa.CSL_Migration_forum_thread_revision ON 

	INSERT INTO opa.CSL_Migration_forum_thread_revision(id,revision, is_latest, forum_thread_id, user_id, content)
	SELECT t1.id,t1.revision, t1.is_latest, t1.forum_thread_id, t1.user_id, t1.content
	FROM #temp_forum_thread_revision t1
	LEFT JOIN opa.CSL_Migration_forum_thread_revision t2 ON t1.id = t2.id
	WHERE t2.id IS NULL
	AND t1.is_comment = 0

	SET IDENTITY_INSERT opa.CSL_Migration_forum_thread_revision OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '6. Forum' source_file,'opa.Staging_ForumPost' source_table,t1.external_id source_id, 'forum_thread_revision' destination_table, t1.id destination_id
	FROM #temp_forum_thread_revision t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'forum_thread_revision'
	WHERE t2.destination_id IS NULL
	AND t1.is_comment = 0

	/***** insert comment start *****/

	INSERT INTO #temp_comment(message,object_model,object_id,created_at,created_by,updated_at,updated_by,external_id)
	SELECT [dbo].[ConvertHTML](ISNULL(t1.content,'')), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, t2.created_at, t2.created_by, 
	t2.updated_at, t2.updated_by, t1.id
	FROM opa.Staging_ForumPost t1
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE t2.is_comment = 1

	SET IDENTITY_INSERT opa.CSL_Migration_comment ON

	INSERT INTO opa.CSL_Migration_comment(id,message,object_model,object_id,created_at,created_by,updated_at,updated_by)
	SELECT t1.id,t1.message,t1.object_model,t1.object_id,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by
	FROM #temp_comment t1
	LEFT JOIN opa.CSL_Migration_comment t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_comment OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '6. Forum' source_file,'opa.Staging_ForumPost' source_table,t1.external_id source_id, 'comment' destination_table, t1.id destination_id
	FROM #temp_comment t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'comment'
	WHERE t2.destination_id IS NULL


	/***** insert file start *****/

	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, 
			RIGHT(attach1_slot, CHARINDEX('/',REVERSE(attach1_slot))-1) file_name,
			t1.name title,
			RIGHT(attach1_slot, CHARINDEX('.',REVERSE(attach1_slot))-1) mime_type,
			t1.attach1_size size, t2.created_at,t2.created_by,t2.updated_at,t2.updated_by,'0' show_in_stream, t1.id external_id, t1.attach1_slot location
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE attach1_slot IS NOT NULL AND attach1_slot <> ''


	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, 
			RIGHT(attach2_slot, CHARINDEX('/',REVERSE(attach2_slot))-1) file_name,
			t1.name title,
			RIGHT(attach2_slot, CHARINDEX('.',REVERSE(attach2_slot))-1) mime_type,
			t1.attach2_size size, t2.created_at,t2.created_by,t2.updated_at,t2.updated_by,'0' show_in_stream, t1.id external_id, t1.attach2_slot location
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE attach2_slot IS NOT NULL AND attach2_slot <> ''


	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, 
			RIGHT(attach3_slot, CHARINDEX('/',REVERSE(attach3_slot))-1) file_name,
			t1.name title,
			RIGHT(attach3_slot, CHARINDEX('.',REVERSE(attach3_slot))-1) mime_type,
			t1.attach3_size size, t2.created_at,t2.created_by,t2.updated_at,t2.updated_by,'0' show_in_stream, t1.id external_id, t1.attach3_slot location
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE attach3_slot IS NOT NULL AND attach3_slot <> ''


	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, 
			RIGHT(attach4_slot, CHARINDEX('/',REVERSE(attach4_slot))-1) file_name,
			t1.name title,
			RIGHT(attach4_slot, CHARINDEX('.',REVERSE(attach4_slot))-1) mime_type,
			t1.attach4_size size, t2.created_at,t2.created_by,t2.updated_at,t2.updated_by,'0' show_in_stream, t1.id external_id, t1.attach4_slot location
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE attach4_slot IS NOT NULL AND attach4_slot <> ''


	INSERT INTO #temp_file(guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,external_id,location)
	SELECT  NEWID(), 'humhub\modules\forum\models\ForumThread' object_model, t2.forum_thread_id object_id, 
			RIGHT(attach5_slot, CHARINDEX('/',REVERSE(attach5_slot))-1) file_name,
			t1.name title,
			RIGHT(attach5_slot, CHARINDEX('.',REVERSE(attach5_slot))-1) mime_type,
			t1.attach4_size size, t2.created_at,t2.created_by,t2.updated_at,t2.updated_by,'0' show_in_stream, t1.id external_id, t1.attach5_slot location
	FROM opa.Staging_ForumPost t1 (NOLOCK)
	JOIN #temp_forum_thread_revision t2 ON t1.id = t2.external_id
	WHERE attach5_slot IS NOT NULL AND attach5_slot <> ''


	SET IDENTITY_INSERT opa.CSL_Migration_file ON

	INSERT INTO opa.CSL_Migration_file(id,guid,object_model,object_id,file_name,title,mime_type,size,created_at,created_by,updated_at,updated_by,show_in_stream,location)
	SELECT t1.id,t1.guid,t1.object_model,t1.object_id,t1.file_name,t1.title,t1.mime_type,t1.size,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by,t1.show_in_stream,t1.location
	FROM #temp_file t1
	LEFT JOIN opa.CSL_Migration_file t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_file OFF

	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '6. Forum' source_file,'opa.Staging_ForumPost' source_table,t1.external_id source_id, 'file' destination_table, t1.id destination_id
	FROM #temp_file t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'file'
	WHERE t2.destination_id IS NULL

	/***** insert activity and content start *****/

	INSERT INTO opa.CSL_Migration_activity(class,module,object_model,object_id)
	SELECT 'humhub\modules\content\activities\ContentCreated' class, 'content' module, 'humhub\modules\forum\models\ForumThread' object_model, t1.id
	FROM #temp_forum_thread t1
	LEFT JOIN opa.CSL_Migration_activity t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\forum\models\ForumThread' AND t2.class = 'humhub\modules\content\activities\ContentCreated'
	WHERE t2.object_id IS NULL



	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\forum\models\ForumThread' object_model, t1.id object_id, '0' visibility, '0' pinned, '0' archived, t1.created_at, t1.created_by,
		   t1.updated_at,t1.updated_by, t2.destination_id contentcontainer_id, t1.created_at stream_sort_date,'default' stream_channel
	FROM #temp_forum_thread t1
	JOIN mig.CSL_ObjectMapping t2 ON t1.group_id = t2.source_id AND t2.source_table = 'opa.Staging_Aggregate' AND t2.destination_table = 'contentcontainer'
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.id = t3.object_id AND t3.object_model = 'humhub\modules\forum\models\ForumThread' AND t3.stream_channel = 'default'
	WHERE t3.object_id IS NULL


	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT NEWID() guid, 'humhub\modules\activity\models\Activity' object_model, t1.id object_id,t2.visibility, t2.pinned, t2.archived, t2.created_at, t2.created_by,
		   t2.updated_at,t2.updated_by, t2.contentcontainer_id, t2.stream_sort_date, 'activity' stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN opa.CSL_Migration_content t2 ON t1.object_id = t2.object_id AND t2.stream_channel = 'default' AND t2.object_model = 'humhub\modules\forum\models\ForumThread'
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.object_id = t3.object_id AND t3.object_model = 'humhub\modules\activity\models\Activity' and t3.stream_channel  = 'activity'
	WHERE  t3.object_id IS NULL


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

        raiserror ('opa.prc_CSL_Migration_Forum_Insert: %d: %s', 16, 1, @error, @message) ;
    end catch

END