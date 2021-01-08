IF OBJECT_ID(N'opa.Staging_ForumThreadCategory', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_ForumThreadCategory(
	id INT NOT NULL,
	name NVARCHAR(1000) NULL,
	forum__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END