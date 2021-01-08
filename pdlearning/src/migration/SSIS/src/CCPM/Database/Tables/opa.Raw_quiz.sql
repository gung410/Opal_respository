IF OBJECT_ID(N'opa.Raw_quiz', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_quiz(
	ID	INT NOT NULL,
	name nvarchar(max) NULL,	
	title	nvarchar(max) NULL,
	description	ntext NULL,	
	type nvarchar(max) NULL,
	relpath	varchar(max) NULL,	
	level nvarchar(max) NULL,	
	owner__id varchar(max) NULL,	
	creator__id	varchar(max) NULL,	
	group__id varchar(max) NULL,
	seq	varchar(max) NULL,	
	config	ntext NULL,	
	content_plate ntext NULL,	
	extra_plate	ntext NULL,	
	qn_hints nvarchar(max) NULL,	
	ans_explanation	nvarchar(max) NULL,	
	max_mark varchar(max) NULL,	
	is_auto_marked varchar(max) NULL,
	show_feedback_after_answer varchar(max) NULL,	
	show_feedback varchar(max) NULL,
	rand_qns varchar(max) NULL,	
	rand_choice	varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END