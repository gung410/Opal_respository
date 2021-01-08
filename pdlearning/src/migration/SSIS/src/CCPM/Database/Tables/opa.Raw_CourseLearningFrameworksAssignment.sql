IF OBJECT_ID(N'opa.Raw_CouresLearningFrameworksAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseLearningFrameworksAssignment(
	ID	INT NOT NULL,
	course__id varchar(max) NULL,
	course_code	varchar(max) NULL,
	lf_code	varchar(max) NULL,
	la_code	varchar(max) NULL,
	lsa_code varchar(max) NULL,
	source varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END