IF OBJECT_ID(N'opa.Staging_BlogComments', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_BlogComments(
	id INT NOT NULL,
	entry__id varchar(1000) NULL,
	comment ntext NULL,
	owner__id Int NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END