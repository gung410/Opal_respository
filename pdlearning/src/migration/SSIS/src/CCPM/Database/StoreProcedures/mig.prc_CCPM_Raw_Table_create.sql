IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Raw_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Raw_Table_create] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Raw_Table_create]
AS
BEGIN


/********************************************** OPAL1 start **************************/

/******************* Repositories start **************************/

IF OBJECT_ID(N'opa.Raw_Resources', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Resources(
	ID	INT NOT NULL,
	title nvarchar(max) NULL,
	repository nvarchar(max) NULL,
	category nvarchar(max) NULL,
	creator__id varchar(max) NULL,
	owner__id varchar(max) NULL,
	division varchar(max) NULL,
	school varchar(max) NULL,
	created_time varchar(max) NULL,
	last_access_time varchar(max) NULL,
	accesscount	varchar(max) NULL, 
	filename nvarchar(max) NULL,
	location nvarchar(max) NULL,
	filesize varchar(max) NULL, 
	description ntext NULL,
	keywords nvarchar(max) NULL,
	resourcetype varchar(max) NULL,
	resourcesubtype varchar(max) NULL,
	language varchar(max) NULL,
	digitalformat varchar(max) NULL,
	copyright nvarchar(max) NULL,
	source nvarchar(max) NULL,
	termsofuse nvarchar(max) NULL,
	expirydate varchar(max) NULL,
	publisher nvarchar(max) NULL,
	details text NULL,
	repository_name nvarchar(max) NULL,
	PRIMARY KEY (ID)
	)
END



/******************* quiz start **************************/
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



/******************* Course start **************************/
IF OBJECT_ID(N'opa.Raw_CourseDetail', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseDetail(
	ID	INT NOT NULL,
	code varchar(max) NULL,
	course_code	varchar(max) NULL,
	class_code	varchar(max) NULL,
	coursetitle	nvarchar(max) NULL,
	description	ntext NULL,
	objective ntext NULL,
	targetaudience ntext NULL,
	sdate varchar(max) NULL,
	edate	varchar(max) NULL,
	duration_hour varchar(max) NULL,
	duration_minutes varchar(max) NULL,
	end_of_reg	varchar(max) NULL,
	traisi_is_listed varchar(max) NULL,
	traisi_course_type	varchar(max) NULL,
	eduLevelPri	varchar(max) NULL,
	eduLevelSec	varchar(max) NULL,
	eduLevelPreU varchar(max) NULL,
	keywords nvarchar(max) NULL,
	trainingagency	nvarchar(max) NULL,
	agg_Id	varchar(max) NULL,
	creator__id	varchar(max) NULL,
	owner__id varchar(max) NULL,
	source	varchar(max) NULL,
	status varchar(max) NULL,
	is_approve	varchar(max) NULL,
	ctime	varchar(max) NULL,
	publisher__id varchar(max) NULL,
	publish_time varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END

IF OBJECT_ID(N'opa.Raw_DevelopmentalRolesAndLearningFrameworkData', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_DevelopmentalRolesAndLearningFrameworkData(
	ID	INT NOT NULL,
	code varchar(max) NULL,
	category varchar(max) NULL,
	description	nvarchar(max) NULL,
	parent_code	varchar(max) NULL,
	status varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END

IF OBJECT_ID(N'opa.Raw_CourseDevelopmentalRoleAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseDevelopmentalRoleAssignment(
	ID	INT NOT NULL,
	course__id varchar(max) NULL,
	course_code	varchar(max) NULL,
	code varchar(max) NULL,
	source varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END

IF OBJECT_ID(N'opa.Raw_CouresLearningFrameworksAssignment', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CouresLearningFrameworksAssignment(
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

IF OBJECT_ID(N'opa.Raw_CourseMaterials', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseMaterials(
	ID	INT NOT NULL,
	title varchar(max) NULL,
	course__id varchar(max) NULL,
	type varchar(max) NULL,
	equiz__id varchar(max) NULL,
	equiz_name nvarchar(max) NULL,
	url	varchar(max) NULL,
	page_content ntext NULL,
	scorm_name nvarchar(max) NULL,
	attach_slot	nvarchar(max) NULL,
	attach_filename	nvarchar(max) NULL,
	attach_size varchar(max) NULL,
	width varchar(max) NULL,
	height varchar(max) NULL,
	description	ntext NULL,
	duration varchar(max) NULL,
	owner__id varchar(max) NULL,
	ctime	varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END


IF OBJECT_ID(N'opa.Raw_CourseItems', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseItems(
	ID	INT NOT NULL,
	title nvarchar(max) NULL,
	course__id varchar(max) NULL,
	partent__id	varchar(max) NULL,
	type varchar(max) NULL,
	seq	varchar(max) NULL,
	item_seq_path varchar(max) NULL,
	level varchar(max) NULL,
	relpath	varchar(max) NULL,
	description	ntext NULL,
	duration varchar(max) NULL,
	enabled_options	varchar(max) NULL,
	enabled_stime varchar(max) NULL,
	completion_req varchar(max) NULL,
	owner__id varchar(max) NULL,
	res__id	varchar(max) NULL,
	res_type varchar(max) NULL,
	PRIMARY KEY (ID)
	)
END


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

/******************* Course end **************************/




/********************************************** OPAL1 end **************************/



/*
/********************************************** TRAISI start **************************/
/******************* CA_COURSE start **************************/
IF OBJECT_ID(N'tra.Raw_CA_COURSE', N'U') IS NULL
BEGIN
	CREATE TABLE tra.Raw_CA_COURSE(
	COURSE_CODE	VARCHAR(27) NOT NULL,
	OWNER_ORG	VARCHAR(6) NOT NULL,
	TEAM_CODE	VARCHAR(5) NULL,
	CLASSIFICATION_CODE	VARCHAR(6) NULL,
	COURSE_NAME	NVARCHAR(400) NULL,
	DURATION_DAYS	FLOAT NULL,
	DURATION_HRS	FLOAT NULL,
	MAIN_TRAINER	VARCHAR(20) NULL,
	T_FOR_ALL_SESSION	CHAR(1) NULL,
	CO_TRAINER	VARCHAR(20) NULL,
	COT_FOR_ALL_SESSION	CHAR(1) NULL,
	PREFERRED_PARTICIPANTS	VARCHAR(500) NULL,
	COURSE_DEFUNCT_DATE	DATETIME NULL,
	RECOMMENDED_CLASS_SIZE	NUMERIC NULL,
	MIN_CLASS_SIZE	NUMERIC NULL,
	MAX_CLASS_SIZE	NUMERIC NULL,
	NEW_COURSE_IND	CHAR(1) NULL,
	PUBLICATION_APPROVAL_IND	CHAR(1) NULL,
	COURSE_PUBLISHED_DATE	DATETIME NULL,
	COURSE_OBJECTIVE	NVARCHAR(2000) NULL,
	INTRO_WRITEUP	VARCHAR(1000) NULL,
	CA_CODE	VARCHAR(20) NULL,
	TSO_CODE	VARCHAR(20) NULL,
	SUGGESTED_TRAINER	VARCHAR(20) NULL,
	COURSE_STATUS	CHAR(2) NULL,
	GEP_IND	CHAR(1) NULL,
	PS21_IND	CHAR(1) NULL,
	COURSE_CONTENTS	NVARCHAR(2000) NULL,
	OTHER_REQUISITES	VARCHAR(500) NULL,
	DEVELOPMENT_IND	CHAR(1) NULL,
	RS_ROOM_LAYOUT	VARCHAR(12) NULL,
	RS_ROOM_TYPE	VARCHAR(8) NULL,
	RS_OTHER_REQUIREMENTS	VARCHAR(300) NULL,
	OVERVIEW	NVARCHAR(400) NULL,
	FOOTNOTE	VARCHAR(200) NULL,
	COMMENT	VARCHAR(300) NULL,
	EVAL_CODE	VARCHAR(15) NULL,
	ASSESSMENT_MODE	VARCHAR(420) NULL,
	VALUE	FLOAT NULL,
	OWNER_TEAM	VARCHAR(5) NULL,
	CORE_COURSE_IND	CHAR(1) NULL,
	SDF_AMT	MONEY NULL,
	NO_PER_DEPT	INT NULL,
	USER_NRIC_NO	VARCHAR(18) NULL,
	ARCHIVE_DATE	DATETIME NULL,
	SELECTED_PER_DEPT	INT NULL,
	TI_COURSE_CODE	VARCHAR(20) NULL,
	ROWGUID	UNIQUEIDENTIFIER NULL,
	AC_COURSE_IND	VARCHAR(1) NULL,
	TLLM_COURSE_IND	VARCHAR(1) NULL,
	COURSE_IND	VARCHAR(1) NULL,
	MULTI_MODULE	INT NULL,
	COURSE_CATEGORY	INT NULL,
	MOE_SPONSORED	INT NULL,
	VALUE_A	FLOAT NULL,
	DIV_1	INT NULL,
	DIV_2	INT NULL,
	DIV_3	INT NULL,
	DIV_4	INT NULL,
	LEVEL_PRIMARY	INT NULL,
	LEVEL_SECONDARY	INT NULL,
	LEVEL_PRE_UNIVERSITY INT NULL,

	PRIMARY KEY (COURSE_CODE,OWNER_ORG)
	)
END


/******************* CA_COURSE end **************************/

/******************* CA_EXTERNAL_COURSE start **************************/

IF OBJECT_ID(N'tra.Raw_CA_EXTERNAL_COURSE', N'U') IS NULL
BEGIN
	CREATE TABLE tra.Raw_CA_EXTERNAL_COURSE(
	COURSE_CODE	VARCHAR(14) NOT NULL,
	COURSE_TITLE	VARCHAR(121) NULL,
	COURSE_START_DATE	DATETIME NULL,
	COURSE_FEE_PER_PAX	FLOAT NULL,
	DURATION_DAYS	FLOAT NULL,
	DURATION_HOURS	FLOAT NULL,
	TC_CODE	VARCHAR(20) NOT NULL,
	TC_ORG_CODE	VARCHAR(6) NOT NULL,
	OWNER_TEAM	VARCHAR(5) NULL,
	ARCHIVE_DATE	DATETIME NULL,
	COURSE_END_DATE	DATETIME NULL,
	DURATION	CHAR(8) NULL,
	NATURE_OF_COURSE	CHAR(1) NULL,
	COURSE_TYPE	CHAR(1) NULL,
	INSTITUTION_CODE	VARCHAR(4) NULL,
	INSTITUTION_CREATOR_CODE	VARCHAR(6) NULL,
	INSTITUTION	VARCHAR(80) NULL,
	COUNTRY	CHAR(3) NULL,
	CLASSIFICATION_CODE	VARCHAR(7) NULL,
	CLASSIFICATION_CREATOR_CODE	VARCHAR(6) NULL,
	CLASSIFICATION_DESCRIPTION	VARCHAR(45) NULL,
	COURSE_CREATOR_CODE	CHAR(3) NULL,
	CAPACITY	VARCHAR(20) NULL,
	WHETHER_SPONSOR	CHAR(1) NULL,
	PAPER_DTL	VARCHAR(200) NULL,
	ORGANISER_DTL	VARCHAR(315) NULL,
	COURSE_DESC	VARCHAR(1000) NULL,
	EVAL_CODE	VARCHAR(15) NULL,
	EVAL_CLOSING_DATE	DATETIME NULL,
	TC_EU_INDICATOR	CHAR(1) NULL,
	CPIS_COURSE_CODE	VARCHAR(6) NULL,
	PRIMARY KEY (COURSE_CODE,TC_CODE,TC_ORG_CODE)
	)
END

/******************* CA_EXTERNAL_COURSE end **************************/
*/

END
