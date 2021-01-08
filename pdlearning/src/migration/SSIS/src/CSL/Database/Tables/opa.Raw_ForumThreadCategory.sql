IF OBJECT_ID(N'opa.Raw_ForumThreadCategory', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_ForumThreadCategory(
	id INT NOT NULL,
	name NVARCHAR(MAX) NULL,
	forum__id VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END