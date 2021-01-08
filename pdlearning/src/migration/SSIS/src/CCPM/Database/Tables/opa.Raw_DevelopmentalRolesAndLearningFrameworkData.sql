IF OBJECT_ID(N'opa.Raw_DevelopmentalRolesAndLearningFrameworkData', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_DevelopmentalRolesAndLearningFrameworkData(
	ID	INT NOT NULL,
	code varchar(max) NULL,
	category varchar(max) NULL,
	description	nvarchar(max) NULL,
	parent_code	varchar(max) NULL,
	status varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END