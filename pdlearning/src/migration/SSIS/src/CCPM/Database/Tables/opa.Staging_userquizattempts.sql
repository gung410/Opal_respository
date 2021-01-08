IF OBJECT_ID(N'opa.Staging_userquizattempts', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_userquizattempts(
	ID	INT NOT NULL,
	user__id	Int NULL,
	equiz__id	Int NULL,
	mark Int NULL,
	max_mark Int NULL,
	score float NULL,
	progress float NULL,
	duration Int NULL,
	attempt_time datetime NULL,
	attempt_complete varchar(50) NULL,
	rand_qns bit NULL,
	all_qns_count text NULL,
	select_qns_count text NULL,
	qns_order text NULL,
	all_qns_order text NULL,
	all_qns_level_type	text NULL,
	seq	Int NULL,
	PRIMARY KEY (ID)
	)
END