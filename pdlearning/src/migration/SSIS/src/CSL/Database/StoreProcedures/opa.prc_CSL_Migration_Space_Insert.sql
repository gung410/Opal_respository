IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_Space_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_Space_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CSL_Migration_Space_Insert
AS
BEGIN

BEGIN TRANSACTION  
	 
	IF OBJECT_ID('tempdb.dbo.#temp_contentcontainer', 'U') IS NOT NULL
	DROP TABLE #temp_contentcontainer; 
	CREATE TABLE #temp_contentcontainer(
	  id INT NOT NULL IDENTITY(1000,1),
	  guid CHAR(36) NOT NULL,
	  class CHAR(60) NOT NULL,
	  pk INT DEFAULT NULL,
	  owner_user_id INT DEFAULT NULL,
	  external_id VARCHAR(255) NOT NULL,
	PRIMARY KEY (id)
	)

	IF OBJECT_ID('tempdb.dbo.#temp_space', 'U') IS NOT NULL
	DROP TABLE #temp_space; 
	CREATE TABLE #temp_space(
	  id INT NOT NULL IDENTITY(1000,1),
	  guid VARCHAR(45) DEFAULT NULL,
	  name NVARCHAR(1000) NOT NULL,
	  description NTEXT,
	  join_policy TINYINT DEFAULT NULL,
	  visibility TINYINT DEFAULT NULL,
	  status TINYINT NOT NULL DEFAULT '1',
	  tags TEXT,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  ldap_dn VARCHAR(255) DEFAULT NULL,
	  auto_add_new_members INT DEFAULT NULL,
	  contentcontainer_id INT DEFAULT NULL,
	  default_content_visibility TINYINT DEFAULT NULL,
	  color VARCHAR(7) DEFAULT NULL,
	  members_can_leave INT DEFAULT '1',
	  url VARCHAR(1000) DEFAULT NULL,
	  external_id VARCHAR(255) NOT NULL,
	PRIMARY KEY (id)
	)

	INSERT INTO #temp_contentcontainer(guid,class,pk,owner_user_id,external_id)
	SELECT NEWID(), 'humhub\modules\space\models\Space' class, '0' pk, t2.Opal2ID owner_user_id, t1.id external_id
	FROM opa.Staging_Aggregate t1
	JOIN opa.Staging_User t2 ON t1.owner__id = t2.Id
	
	SET IDENTITY_INSERT opa.CSL_Migration_contentcontainer ON 

	INSERT INTO opa.CSL_Migration_contentcontainer(id,guid,class,pk,owner_user_id)
	SELECT t1.id,t1.guid,t1.class,t1.pk,t1.owner_user_id
	FROM #temp_contentcontainer t1
	LEFT JOIN opa.CSL_Migration_contentcontainer t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_contentcontainer OFF 


	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '2. Aggregate (Courses and Groups)' source_file,'opa.Staging_Aggregate' source_table,t1.external_id source_id, 'contentcontainer' destination_table, t1.id destination_id
	FROM #temp_contentcontainer t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'contentcontainer'
	WHERE t2.destination_id IS NULL

	INSERT INTO #temp_space(guid,name,description,join_policy,visibility,status,tags,created_at,created_by,updated_at,updated_by,
							ldap_dn,auto_add_new_members,contentcontainer_id,default_content_visibility,color,members_can_leave,url,external_id)
	SELECT	t2.guid guid, t1.label name, [dbo].[ConvertHTML](ISNULL(t1.about,'')) description, 
			CASE
					WHEN t1.acs = 'private' THEN 0
					WHEN t1.acs = 'public_limitedacs' THEN 1
					ELSE 2
				END AS join_policy,
		    '1' visibility,
			CAST(	CASE
					WHEN t1.status = 'active'
						THEN 1
					ELSE 0
				END AS TINYINT) as status,
			NULL tags, CAST(t1.ctime AS DATETIME) created_at, t3.Opal2ID created_by, CAST(t1.ctime AS DATETIME) updated_at, t3.Opal2ID updated_by,
			NULL ldap_dn, NULL auto_add_new_members, t2.id contentcontainer_id, NULL default_content_visibility, NULL color, '1' members_can_leave,
			t1.label url, t1.id external_id
	FROM opa.Staging_Aggregate t1
	JOIN #temp_contentcontainer t2 ON t1.id = t2.external_id
	JOIN opa.Staging_User t3 ON t1.owner__id = t3.Id
	
	SET IDENTITY_INSERT opa.CSL_Migration_space ON

	INSERT INTO opa.CSL_Migration_space( id,guid,name,description,join_policy,visibility,status,tags,created_at,created_by,updated_at,updated_by,
											ldap_dn,auto_add_new_members,contentcontainer_id,default_content_visibility,color,members_can_leave,url)
	SELECT t1.id,t1.guid,t1.name,t1.description,t1.join_policy,t1.visibility,t1.status,t1.tags,t1.created_at,t1.created_by,t1.updated_at,t1.updated_by,
		   t1.ldap_dn,t1.auto_add_new_members,t1.contentcontainer_id,t1.default_content_visibility,t1.color,t1.members_can_leave,t1.guid 'url'
	FROM #temp_space t1
	LEFT JOIN opa.CSL_Migration_space t2 ON t1.id = t2.id
	WHERE t2.id IS NULL

	SET IDENTITY_INSERT opa.CSL_Migration_space OFF 

	UPDATE opa.CSL_Migration_contentcontainer 
	SET pk = t2.id
	FROM opa.CSL_Migration_contentcontainer t1 JOIN opa.CSL_Migration_space t2  ON t1.id = t2.contentcontainer_id

	
	INSERT INTO mig.CSL_ObjectMapping(source_file,source_table,source_id ,destination_table,destination_id)
	SELECT '2. Aggregate (Courses and Groups)' source_file,'opa.Staging_Aggregate' source_table,t1.external_id source_id, 'space' destination_table, t1.id destination_id
	FROM #temp_space t1
	LEFT JOIN mig.CSL_ObjectMapping t2 ON t1.id = t2.destination_id AND t2.destination_table = 'space'
	WHERE t2.destination_id IS NULL

	INSERT INTO opa.CSL_Migration_activity(class,module,object_model,object_id)
	SELECT 'humhub\modules\space\activities\Created' class, 'space' module, 'humhub\modules\space\models\Space' object_model, t1.id object_id
	FROM #temp_space t1
	LEFT JOIN opa.CSL_Migration_activity t2 ON t1.id = t2.object_id AND t2.object_model = 'humhub\modules\space\models\Space' AND t2.class = 'humhub\modules\space\activities\Created'
	WHERE t2.object_id IS NULL
	
	INSERT INTO opa.CSL_Migration_content(guid,object_model,object_id,visibility,pinned,archived,created_at,created_by,
											 updated_at,updated_by,contentcontainer_id,stream_sort_date,stream_channel)
	SELECT  NEWID() guid,'humhub\modules\activity\models\Activity' object_model, t1.id object_id, '1' visibility, '0' pinned, '0' archived, t2.created_at,t2.created_by,
			t2.updated_at,t2.updated_by, t2.contentcontainer_id, t2.created_at stream_sort_date,'default'stream_channel
	FROM opa.CSL_Migration_activity t1
	JOIN #temp_space t2 ON t1.object_id = t2.id
	LEFT JOIN opa.CSL_Migration_content t3 ON t1.id = t3.object_id AND t3.object_model = 'humhub\modules\activity\models\Activity' AND t3.stream_channel = 'default'
	WHERE t3.object_id IS NULL


	INSERT INTO opa.CSL_Migration_space_membership(space_id,user_id,originator_user_id,status,request_message,last_visit,created_at,created_by,
													  updated_at,updated_by,group_id,show_at_dashboard,can_cancel_membership,send_notifications)
	SELECT t3.id space_id, t2.Opal2ID user_id, NULL originator_user_id, 3 status, NULL request_message, NULL last_visit, t3.created_at,t3.created_by,
		   t3.updated_at,t3.updated_by, t1.role group_id, 1 show_at_dashboard, 1 can_cancel_membership,0 send_notifications
	FROM opa.Staging_CourseAndGroupParticipants t1 
	JOIN opa.Staging_User t2 ON t1.user_id = t2.ID
	JOIN #temp_space t3 ON t1.aggregate_id = t3.external_id
	LEFT JOIN opa.CSL_Migration_space_membership t4 ON t3.id = t4.space_id AND t2.Opal2ID  = t4.user_id
	WHERE t4.user_id IS NULL


	INSERT INTO opa.CSL_Migration_space_membership(space_id,user_id,originator_user_id,status,request_message,last_visit,created_at,created_by,
													  updated_at,updated_by,group_id,show_at_dashboard,can_cancel_membership,send_notifications)
	SELECT t1.id space_id, t1.created_by user_id, NULL originator_user_id, 3 status, NULL request_message, NULL last_visit, t1.created_at,t1.created_by,
		   t1.updated_at,t1.updated_by, 'admin' group_id, 1 show_at_dashboard, 1 can_cancel_membership,0 send_notifications
	FROM #temp_space t1 
	JOIN opa.Staging_User t2 ON t1.created_by = t2.Opal2ID
	-- JOIN opa.Staging_CourseAndGroupParticipants t3 ON t1.external_id = t3.aggregate_id AND t2.ID = t3.user_id
	LEFT JOIN opa.CSL_Migration_space_membership t4 ON t1.id = t4.space_id AND t1.created_by = t4.user_id
	WHERE t4.user_id IS NULL

	INSERT INTO opa.CSL_Migration_content_tag(name,module_id,contentcontainer_id,type,parent_id,color)
	SELECT 'Blog' name, 'topic' module_id, t1.id contentcontainer_id, 'humhub\modules\topic\models\Topic' type, NULL parent_id, NULL color
	FROM #temp_contentcontainer t1
	LEFT JOIN opa.CSL_Migration_content_tag t2 ON t1.id = t2.contentcontainer_id AND t2.name = 'Blog'
	WHERE t2.contentcontainer_id IS NULL

	INSERT INTO opa.CSL_Migration_content_tag(name,module_id,contentcontainer_id,type,parent_id,color)
	SELECT 'Wall Post' name, 'topic' module_id, t1.id contentcontainer_id, 'humhub\modules\topic\models\Topic' type, NULL parent_id, NULL color
	FROM #temp_contentcontainer t1
	LEFT JOIN opa.CSL_Migration_content_tag t2 ON t1.id = t2.contentcontainer_id AND t2.name = 'Wall Post'
	WHERE t2.contentcontainer_id IS NULL

IF (@@Error <> 0) 
   BEGIN          
        ROLLBACK TRANSACTION       
   END 
   ELSE 
        COMMIT TRANSACTION
	
END
GO
