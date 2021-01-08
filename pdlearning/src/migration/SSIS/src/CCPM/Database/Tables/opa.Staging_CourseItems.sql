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