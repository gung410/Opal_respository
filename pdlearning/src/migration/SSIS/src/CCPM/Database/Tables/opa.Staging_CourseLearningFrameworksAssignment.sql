IF OBJECT_ID(N'opa.Staging_CouresLearningFrameworksAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseLearningFrameworksAssignment(
	ID	INT NOT NULL,
	course__id Int NULL,
	course_code	varchar(255) NULL,
	lf_code	varchar(255) NULL,
	la_code	varchar(255) NULL,
	lsa_code varchar(255) NULL,
	source varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END