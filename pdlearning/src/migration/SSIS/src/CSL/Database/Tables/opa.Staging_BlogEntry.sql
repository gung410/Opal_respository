IF OBJECT_ID(N'opa.Staging_BlogEntry', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_BlogEntry(
	id INT NOT NULL,
	blog__id varchar(1000) NULL,
	title nvarchar(1000) NULL,
	category nvarchar(1000) NULL,
	content  ntext NULL,
	owner__id Int NULL,
	stime datetime NULL,
	status varchar(1000) NULL,
	res_slot nvarchar(1000) NULL,
	res_file nvarchar(1000) NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END