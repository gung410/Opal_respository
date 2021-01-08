IF OBJECT_ID(N'opa.Staging_quiz', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_quiz(
	ID	INT NOT NULL,
	name nvarchar(1000) NULL,	
	title	nvarchar(max) NULL,
	description	ntext NULL,	
	type nvarchar(50) NULL,
	relpath	varchar(50) NULL,	
	level nvarchar(50) NULL,	
	owner__id int NULL,	
	creator__id	int NULL,	
	group__id int NULL,
	seq	varchar(50) NULL,	
	config	ntext NULL,	
	content_plate ntext NULL,	
	extra_plate	ntext NULL,	
	qn_hints nvarchar(max) NULL,	
	ans_explanation	nvarchar(max) NULL,	
	max_mark tinyint NULL,	
	is_auto_marked bit NULL,
	show_feedback_after_answer varchar(256) NULL,	
	show_feedback varchar(256) NULL,
	rand_qns bit NULL,	
	rand_choice	bit NULL,
	ctime datetime NULL,
	PRIMARY KEY (ID)
	)

END