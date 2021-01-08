IF OBJECT_ID(N'opa.CSL_Migration_content_tag_relation', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_content_tag_relation (
	  id int NOT NULL IDENTITY(1,1),
	  content_id int NOT NULL,
	  tag_id int NOT NULL,
	  PRIMARY KEY(id)
	)
END