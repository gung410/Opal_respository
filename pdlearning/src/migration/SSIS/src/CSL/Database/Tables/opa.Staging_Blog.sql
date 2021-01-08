IF OBJECT_ID(N'opa.Staging_Blog', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Blog(
	id INT NOT NULL,
	title nvarchar(1000) NULL,
	description	ntext NULL,
	owner__id Int NULL,
	group__id Int NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END