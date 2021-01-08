IF OBJECT_ID(N'opa.Staging_CourseDevelopmentalRoleAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseDevelopmentalRoleAssignment(
	ID	INT NOT NULL,
	course__id Int NULL,
	course_code	varchar(255) NULL,
	code varchar(255) NULL,
	source varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END