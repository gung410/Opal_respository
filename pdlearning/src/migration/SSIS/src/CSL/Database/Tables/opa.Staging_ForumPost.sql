IF OBJECT_ID(N'opa.Staging_ForumPost', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_ForumPost(
	id INT NOT NULL,
	thread__id VARCHAR(1000) NULL,
	parent__id INT NULL,
	first_post__id INT NULL,
	owner__id INT NULL,
	name NTEXT NULL,
	content NTEXT NULL,
	status VARCHAR(1000) NULL,
	attach1_slot NVARCHAR(1000) NULL,
	attach1_filename NVARCHAR(1000) NULL,
	attach1_size INT NULL,
	attach2_slot NVARCHAR(1000) NULL,
	attach2_filename NVARCHAR(1000) NULL,
	attach2_size INT NULL,
	attach3_slot NVARCHAR(1000) NULL,
	attach3_filename NVARCHAR(1000) NULL,
	attach3_size INT NULL,
	attach4_slot NVARCHAR(1000) NULL,
	attach4_filename NVARCHAR(1000) NULL,
	attach4_size INT NULL,
	attach5_slot NVARCHAR(1000) NULL,
	attach5_filename NVARCHAR(1000) NULL,
	attach5_size INT NULL,
	last_edit_uid INT NULL,
	last_edit_time DATETIME NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END