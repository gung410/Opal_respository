IF OBJECT_ID(N'opa.Raw_ForumPost', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_ForumPost(
	id INT NOT NULL,
	thread__id VARCHAR(MAX) NULL,
	parent__id VARCHAR(MAX) NULL,
	first_post__id VARCHAR(MAX) NULL,
	owner__id VARCHAR(MAX) NULL,
	name NTEXT NULL,
	content NTEXT NULL,
	status VARCHAR(MAX) NULL,
	attach1_slot NVARCHAR(MAX) NULL,
	attach1_filename NVARCHAR(MAX) NULL,
	attach1_size VARCHAR(MAX) NULL,
	attach2_slot NVARCHAR(MAX) NULL,
	attach2_filename NVARCHAR(MAX) NULL,
	attach2_size VARCHAR(MAX) NULL,
	attach3_slot NVARCHAR(MAX) NULL,
	attach3_filename NVARCHAR(MAX) NULL,
	attach3_size VARCHAR(MAX) NULL,
	attach4_slot NVARCHAR(MAX) NULL,
	attach4_filename NVARCHAR(MAX) NULL,
	attach4_size VARCHAR(MAX) NULL,
	attach5_slot NVARCHAR(MAX) NULL,
	attach5_filename NVARCHAR(MAX) NULL,
	attach5_size VARCHAR(MAX) NULL,
	last_edit_uid VARCHAR(MAX) NULL,
	last_edit_time VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END