IF OBJECT_ID(N'opa.Staging_ForumThread', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_ForumThread(
	id INT NOT NULL,
	forum__id INT NULL,
	category__id INT NULL,
	owner__id INT NULL,
	title NVARCHAR(MAX) NULL,
	description	NTEXT NULL,
	status VARCHAR(1000) NULL,
	last_post__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END