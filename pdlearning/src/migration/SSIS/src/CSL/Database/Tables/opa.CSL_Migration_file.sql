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