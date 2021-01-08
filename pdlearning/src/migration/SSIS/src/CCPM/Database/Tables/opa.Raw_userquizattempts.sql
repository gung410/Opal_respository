IF OBJECT_ID(N'opa.Raw_userquizattempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_userquizattempts(
	ID	INT NOT NULL,
	user__id	varchar(max) NULL,
	equiz__id	varchar(max) NULL,
	mark varchar(max) NULL,
	max_mark varchar(max) NULL,
	score varchar(max) NULL,
	progress varchar(max) NULL,
	duration varchar(max) NULL,
	attempt_time varchar(max) NULL,
	attempt_complete varchar(max) NULL,
	rand_qns varchar(max) NULL,
	all_qns_count text NULL,
	select_qns_count text NULL,
	qns_order text NULL,
	all_qns_order text NULL,
	all_qns_level_type	text NULL,
	seq	varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END