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