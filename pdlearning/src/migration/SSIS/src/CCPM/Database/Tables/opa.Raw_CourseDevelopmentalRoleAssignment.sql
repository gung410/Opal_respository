IF OBJECT_ID(N'opa.Raw_CourseDevelopmentalRoleAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseDevelopmentalRoleAssignment(
	ID	INT NOT NULL,
	course__id varchar(max) NULL,
	course_code	varchar(max) NULL,
	code varchar(max) NULL,
	source varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END