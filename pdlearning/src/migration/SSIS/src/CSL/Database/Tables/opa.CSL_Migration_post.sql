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