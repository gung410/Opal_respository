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