IF OBJECT_ID(N'opa.Raw_Aggregate', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Aggregate(
	id INT NOT NULL,
	label nvarchar(max) NULL,
	type varchar(max) NULL,
	about ntext NULL,
	keywords nvarchar(max) NULL,
	intendedfor varchar(max) NULL,
	parent__id varchar(max) NULL,
	acs varchar(max) NULL,
	status varchar(max) NULL,
	creator__id	varchar(max) NULL,
	owner__id varchar(max) NULL,
	ctime varchar(max)	 NULL,
	PRIMARY KEY (id)
	)
END
