IF OBJECT_ID(N'opa.Raw_Blog', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Blog(
	id INT NOT NULL,
	title nvarchar(max) NULL,
	description	ntext NULL,
	owner__id varchar(max) NULL,
	group__id varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END