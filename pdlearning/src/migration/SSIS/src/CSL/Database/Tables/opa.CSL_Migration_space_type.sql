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