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