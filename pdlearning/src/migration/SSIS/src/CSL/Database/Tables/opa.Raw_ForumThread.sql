IF OBJECT_ID(N'opa.Raw_ForumThread', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_ForumThread(
	id INT NOT NULL,
	forum__id VARCHAR(MAX) NULL,
	category__id VARCHAR(MAX) NULL,
	owner__id VARCHAR(MAX) NULL,
	title NVARCHAR(MAX) NULL,
	description	NTEXT NULL,
	status VARCHAR(MAX) NULL,
	last_post__id VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END