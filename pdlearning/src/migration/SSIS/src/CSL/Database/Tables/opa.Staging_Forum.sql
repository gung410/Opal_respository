IF OBJECT_ID(N'opa.Staging_Forum', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Forum(
	id INT NOT NULL,
	name NVARCHAR(1000) NULL,
	group_id INT NULL,
	parent__id INT NULL,
	owner__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END