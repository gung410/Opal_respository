IF OBJECT_ID(N'opa.Staging_DevelopmentalRolesAndLearningFrameworkData', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_DevelopmentalRolesAndLearningFrameworkData(
	ID	INT NOT NULL,
	code varchar(255) NULL,
	category varchar(255) NULL,
	description	nvarchar(max) NULL,
	parent_code	varchar(255) NULL,
	status varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END