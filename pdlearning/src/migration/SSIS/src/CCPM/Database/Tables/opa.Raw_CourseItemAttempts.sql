IF OBJECT_ID(N'opa.Raw_CourseItemAttempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseItemAttempts(
	ID	INT NOT NULL,
	user__id varchar(max) NULL,
	course__id varchar(max) NULL,
	item__id varchar(max) NULL,
	seq	varchar(max) NULL,
	completion_status varchar(max) NULL,
	progress_measure varchar(max) NULL,
	score varchar(max) NULL,
	stime varchar(max) NULL,
	etime varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END