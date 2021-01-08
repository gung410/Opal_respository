IF OBJECT_ID(N'opa.Staging_Aggregate', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Aggregate(
	id INT NOT NULL,
	label nvarchar(1000) NULL,
	type varchar(1000) NULL,
	about ntext NULL,
	keywords nvarchar(1000) NULL,
	intendedfor varchar(1000) NULL,
	parent__id Int NULL,
	acs varchar(1000) NULL,
	status varchar(1000) NULL,
	creator__id	Int NULL,
	owner__id Int NULL,
	ctime datetime	 NULL,
	PRIMARY KEY (id)
	)
END