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