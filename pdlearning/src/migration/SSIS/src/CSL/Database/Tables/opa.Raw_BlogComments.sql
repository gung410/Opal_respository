IF OBJECT_ID(N'opa.Raw_BlogComments', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_BlogComments(
	id INT NOT NULL,
	entry__id varchar(max) NULL,
	comment ntext NULL,
	owner__id varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END