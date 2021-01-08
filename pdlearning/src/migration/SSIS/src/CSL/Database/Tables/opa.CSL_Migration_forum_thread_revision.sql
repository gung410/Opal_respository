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