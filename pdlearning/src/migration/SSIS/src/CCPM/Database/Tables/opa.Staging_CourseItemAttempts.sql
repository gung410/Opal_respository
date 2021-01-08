IF OBJECT_ID(N'opa.Staging_CourseItemAttempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseItemAttempts(
	ID	INT NOT NULL,
	user__id Int NULL,
	course__id Int NULL,
	item__id Int NULL,
	seq	Int NULL,
	completion_status varchar(255) NULL,
	progress_measure float NULL,
	score float NULL,
	stime datetime NULL,
	etime datetime NULL,
	PRIMARY KEY (ID)
	)
END