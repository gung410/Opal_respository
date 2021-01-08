IF OBJECT_ID(N'opa.Raw_BlogEntry', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_BlogEntry(
	id INT NOT NULL,
	blog__id varchar(max) NULL,
	title nvarchar(max) NULL,
	category nvarchar(max) NULL,
	content  ntext NULL,
	owner__id varchar(max) NULL,
	stime varchar(max) NULL,
	status varchar(max) NULL,
	res_slot nvarchar(max) NULL,
	res_file nvarchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END