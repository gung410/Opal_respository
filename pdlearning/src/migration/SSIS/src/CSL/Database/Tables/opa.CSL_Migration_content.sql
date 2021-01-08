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

GO 

DBCC CHECKIDENT ('opa.CSL_Migration_content', RESEED, 3000)