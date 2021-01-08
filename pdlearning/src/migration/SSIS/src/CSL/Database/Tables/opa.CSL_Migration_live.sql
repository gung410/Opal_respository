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