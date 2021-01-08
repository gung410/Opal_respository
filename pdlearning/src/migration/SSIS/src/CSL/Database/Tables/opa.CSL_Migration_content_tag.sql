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

GO

DBCC CHECKIDENT ('opa.CSL_Migration_content_tag', RESEED, 3000)