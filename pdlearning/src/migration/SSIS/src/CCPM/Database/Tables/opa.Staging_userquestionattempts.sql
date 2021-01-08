IF OBJECT_ID(N'opa.Staging_userquestionattempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_userquestionattempts(
	ID	INT NOT NULL,
	equizid	Int NULL,
	attempid Int NULL,
	response text NULL,
	is_correct varchar(50) NULL,
	mark smallint NULL,
	max_mark smallint NULL,
	ori_mark smallint NULL,
	score float NULL,
	progress float NULL,
	total_duration Int NULL,
	last_attempted datetime NULL,
	PRIMARY KEY (ID)
	)
END