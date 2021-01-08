IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Staging_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Staging_Table_create] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Staging_Table_create
AS
BEGIN


/********************************************** OPAL1 start **************************/


IF OBJECT_ID(N'opa.Staging_Resources', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Resources(
	ID	INT NOT NULL,
	title nvarchar(max) NULL,
	repository nvarchar(max) NULL,
	category nvarchar(max) NULL,
	creator__id varchar(255) NULL,
	owner__id varchar(255) NULL,
	division varchar(max) NULL,
	school varchar(max) NULL,
	created_time datetime NULL,
	last_access_time datetime NULL,
	accesscount	Int NULL, 
	filename nvarchar(max) NULL,
	location nvarchar(max) NULL,
	filesize Int NULL, 
	description ntext NULL,
	keywords nvarchar(max) NULL,
	resourcetype varchar(1000) NULL,
	resourcesubtype varchar(1000) NULL,
	language varchar(255) NULL,
	digitalformat varchar(max) NULL,
	copyright nvarchar(max) NULL,
	source nvarchar(max) NULL,
	termsofuse nvarchar(max) NULL,
	expirydate date NULL,
	publisher nvarchar(1000) NULL,
	details text NULL,
	repository_name nvarchar(255) NULL,
	PRIMARY KEY (ID)
	)
END


/******************* quiz start **************************/

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
	show_feedback_after_answer bit NULL,	
	show_feedback bit NULL,
	rand_qns bit NULL,	
	rand_choice	bit NULL,
	ctime datetime NULL,
	PRIMARY KEY (ID)
	)

END


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


/******************* course start **************************/

IF OBJECT_ID(N'opa.Staging_CourseDetail', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseDetail(
	ID	INT NOT NULL,
	code varchar(255) NULL,
	course_code	varchar(255) NULL,
	class_code	varchar(255) NULL,
	coursetitle	nvarchar(255) NULL,
	description	ntext NULL,
	objective ntext NULL,
	targetaudience ntext NULL,
	sdate date NULL,
	edate	date NULL,
	duration_hour Int NULL,
	duration_minutes Int NULL,
	end_of_reg	date NULL,
	traisi_is_listed varchar(50) NULL,
	traisi_course_type	varchar(255) NULL,
	eduLevelPri	tinyint NULL,
	eduLevelSec	tinyint NULL,
	eduLevelPreU tinyint NULL,
	keywords nvarchar(255) NULL,
	trainingagency	nvarchar(255) NULL,
	agg_Id	Int NULL,
	creator__id	Int NULL,
	owner__id Int NULL,
	source	varchar(255) NULL,
	status varchar(255) NULL,
	is_approve	varchar(2) NULL,
	ctime	datetime NULL,
	publisher__id Int NULL,
	publish_time datetime NULL,
	PRIMARY KEY (ID)
	)
END


IF OBJECT_ID(N'opa.Staging_DevelopmentalRolesAndLearningFrameworkData', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_DevelopmentalRolesAndLearningFrameworkData(
	ID	INT NOT NULL,
	code varchar(255) NULL,
	category varchar(255) NULL,
	description	nvarchar(max) NULL,
	parent_code	varchar(255) NULL,
	status varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END

IF OBJECT_ID(N'opa.Staging_CourseDevelopmentalRoleAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseDevelopmentalRoleAssignment(
	ID	INT NOT NULL,
	course__id Int NULL,
	course_code	varchar(255) NULL,
	code varchar(255) NULL,
	source varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END

IF OBJECT_ID(N'opa.Staging_CouresLearningFrameworksAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CouresLearningFrameworksAssignment(
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

IF OBJECT_ID(N'opa.Staging_CourseMaterials', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseMaterials(
	ID	INT NOT NULL,
	title varchar(255) NULL,
	course__id Int NULL,
	type varchar(255) NULL,
	equiz__id Int NULL,
	equiz_name nvarchar(255) NULL,
	url	varchar(255) NULL,
	page_content ntext NULL,
	scorm_name nvarchar(255) NULL,
	attach_slot	nvarchar(255) NULL,
	attach_filename	nvarchar(255) NULL,
	attach_size Int NULL,
	width Int NULL,
	height Int NULL,
	description	ntext NULL,
	duration Int NULL,
	owner__id Int NULL,
	ctime	datetime NULL,
	PRIMARY KEY (ID)
	)
END


IF OBJECT_ID(N'opa.Staging_CourseItems', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseItems(
	ID	INT NOT NULL,
	title nvarchar(255) NULL,
	course__id Int NULL,
	partent__id	Int NULL,
	type varchar(255) NULL,
	seq	Int NULL,
	item_seq_path varchar(255) NULL,
	level tinyint NULL,
	relpath	varchar(255) NULL,
	description	ntext NULL,
	duration Int NULL,
	enabled_options	varchar(255) NULL,
	enabled_stime datetime NULL,
	completion_req varchar(255) NULL,
	owner__id Int NULL,
	res__id	Int NULL,
	res_type varchar(255) NULL,
	PRIMARY KEY (ID)
	)
END


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


END