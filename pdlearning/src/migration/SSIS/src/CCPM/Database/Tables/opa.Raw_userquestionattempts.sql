IF OBJECT_ID(N'opa.Raw_userquestionattempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_userquestionattempts(
	ID	INT NOT NULL,
	equizid	varchar(max) NULL,
	attempid varchar(max) NULL,
	response text NULL,
	is_correct varchar(max) NULL,
	mark varchar(max) NULL,
	max_mark varchar(max) NULL,
	ori_mark varchar(max) NULL,
	score varchar(max) NULL,
	progress varchar(max) NULL,
	total_duration varchar(max) NULL,
	last_attempted varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END