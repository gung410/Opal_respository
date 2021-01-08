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