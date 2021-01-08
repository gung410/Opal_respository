IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Migration_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Migration_Table_create] AS ')
GO
ALTER PROCEDURE [opa].[prc_CSL_Migration_Table_create]
AS
BEGIN

IF OBJECT_ID(N'mig.CSL_ObjectMapping', N'U') IS NULL
BEGIN
	CREATE TABLE mig.CSL_ObjectMapping(
	  id int NOT NULL IDENTITY(1,1),
	  source_file VARCHAR(100) NOT NULL,
	  source_table VARCHAR(100) NOT NULL,
	  source_id VARCHAR(100) ,
	  destination_table VARCHAR(100) ,
	  destination_id VARCHAR(100) NOT NULL,
	  PRIMARY KEY (id)
	)
END


IF OBJECT_ID(N'opa.CSL_Migration_activity', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_activity(
	  id int NOT NULL IDENTITY(1,1),
	  class VARCHAR(100) NOT NULL,
	  module VARCHAR(100) DEFAULT '',
	  object_model VARCHAR(100) DEFAULT '',
	  object_id int NOT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_comment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_comment(
	  id INT NOT NULL IDENTITY(1,1),
	  message NTEXT,
	  object_model VARCHAR(100) NOT NULL,
	  object_id INT NOT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_content', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_content(
	  id INT NOT NULL IDENTITY(1,1),
	  guid VARCHAR(45) NOT NULL,
	  object_model VARCHAR(100) NOT NULL,
	  object_id INT NOT NULL,
	  visibility TINYINT DEFAULT NULL,
	  pinned TINYINT DEFAULT NULL,
	  archived TEXT,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  contentcontainer_id INT DEFAULT NULL,
	  stream_sort_date DATETIME DEFAULT NULL,
	  stream_channel CHAR(15) DEFAULT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_contentcontainer', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_contentcontainer(
	  id INT NOT NULL IDENTITY(1,1),
	  guid CHAR(36) NOT NULL,
	  class CHAR(60) NOT NULL,
	  pk INT DEFAULT NULL,
	  owner_user_id INT DEFAULT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_file', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_file(
	  id INT NOT NULL IDENTITY(1,1),
	  guid VARCHAR(45) DEFAULT NULL,
	  object_model VARCHAR(100) DEFAULT '',
	  object_id VARCHAR(100) DEFAULT '',
	  file_name NVARCHAR(255) DEFAULT NULL,
	  title NVARCHAR(255) DEFAULT NULL,
	  mime_type VARCHAR(150) DEFAULT NULL,
	  size VARCHAR(45) DEFAULT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  show_in_stream TINYINT DEFAULT '1',
	  location VARCHAR(255) DEFAULT ''
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_live', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_live(
	  id INT NOT NULL IDENTITY(1,1),
	  contentcontainer_id INT DEFAULT NULL,
	  visibility INT DEFAULT NULL,
	  serialized_data TEXT NOT NULL,
	  created_at INT NOT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_post', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_post(
	  id INT NOT NULL IDENTITY(1,1),
	  message_2trash TEXT,
	  message NTEXT,
	  url VARCHAR(255) DEFAULT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL
	PRIMARY KEY (id)
	)
END



IF OBJECT_ID(N'opa.CSL_Migration_space', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_space(
	  id INT NOT NULL IDENTITY(1,1),
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
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_space_membership', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_space_membership(
	  space_id INT NOT NULL,
	  user_id INT NOT NULL,
	  originator_user_id VARCHAR(45) DEFAULT NULL,
	  status TINYINT DEFAULT NULL,
	  request_message TEXT,
	  last_visit DATETIME DEFAULT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  group_id VARCHAR(255) DEFAULT 'member',
	  show_at_dashboard TINYINT DEFAULT '1',
	  can_cancel_membership INT DEFAULT '1',
	  send_notifications TINYINT DEFAULT '0',
	  id INT NOT NULL IDENTITY(1,1),
	  authclient_id VARCHAR(20) DEFAULT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_space_type', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_space_type(
	  id INT NOT NULL IDENTITY(1,1),
	  title VARCHAR(255) NOT NULL,
	  item_title VARCHAR(255) NOT NULL,
	  sort_key INT NOT NULL DEFAULT '100',
	  show_in_directory TINYINT NOT NULL DEFAULT '1',
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_user_follow', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_user_follow(
	  id INT NOT NULL IDENTITY(1,1),
	  object_model VARCHAR(100) NOT NULL,
	  object_id INT NOT NULL,
	  user_id INT NOT NULL,
	  send_notifications TINYINT DEFAULT '1',
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_content_tag', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_content_tag(
	  id int NOT NULL IDENTITY(1,1),
	  name varchar(100) NOT NULL,
	  module_id varchar(100) NOT NULL,
	  contentcontainer_id int DEFAULT NULL,
	  type varchar(100) DEFAULT NULL,
	  parent_id int DEFAULT NULL,
	  color varchar(7) DEFAULT NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.CSL_Migration_content_tag_relation', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_content_tag_relation (
	  id int NOT NULL IDENTITY(1,1),
	  content_id int NOT NULL,
	  tag_id int NOT NULL,
	  PRIMARY KEY(id)
	)
END


IF OBJECT_ID(N'opa.CSL_Migration_forum_thread', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_forum_thread (
	  id INT NOT NULL IDENTITY(1,1),
	  title NVARCHAR(1000) NOT NULL,
	  is_home TINYINT NOT NULL DEFAULT '0',
	  admin_only TINYINT NOT NULL DEFAULT '0',
	  is_category TINYINT DEFAULT '0',
	  parent_thread_id INT DEFAULT NULL,
	  sort_order INT DEFAULT '0',
  	PRIMARY KEY (id)
)
END


IF OBJECT_ID(N'opa.CSL_Migration_forum_thread_revision', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_forum_thread_revision (
	  id INT NOT NULL IDENTITY(1,1),
	  revision BIGINT NOT NULL,
	  is_latest TINYINT NOT NULL DEFAULT '0',
	  forum_thread_id INT NOT NULL,
	  user_id INT NOT NULL,
	  content NTEXT,
  PRIMARY KEY (id)
  )
END



/*
IF OBJECT_ID(N'opa.CSL_Migration_wiki_page', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_wiki_page (
	  id INT NOT NULL IDENTITY(1,1),
	  title VARCHAR(255) NOT NULL,
	  is_home TINYINT NOT NULL DEFAULT '0',
	  admin_only TINYINT NOT NULL DEFAULT '0',
	  is_category TINYINT DEFAULT '0',
	  parent_page_id INT DEFAULT NULL,
	  sort_order INT DEFAULT '0',
  	PRIMARY KEY (id)
)
END


IF OBJECT_ID(N'opa.CSL_Migration_wiki_page_revision', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_wiki_page_revision (
	  id INT NOT NULL IDENTITY(1,1),
	  revision INT NOT NULL,
	  is_latest TINYINT NOT NULL DEFAULT '0',
	  wiki_page_id INT NOT NULL,
	  user_id INT NOT NULL,
	  content TEXT,
  PRIMARY KEY (id)
  )
END

*/

	DBCC CHECKIDENT ('mig.CSL_ObjectMapping', RESEED, 1)
	DBCC CHECKIDENT ('opa.CSL_Migration_activity', RESEED, 3000)  
	DBCC CHECKIDENT ('opa.CSL_Migration_content', RESEED, 3000)  
	DBCC CHECKIDENT ('opa.CSL_Migration_content_tag', RESEED, 3000)  
	DBCC CHECKIDENT ('opa.CSL_Migration_user_follow', RESEED, 3000)  
END