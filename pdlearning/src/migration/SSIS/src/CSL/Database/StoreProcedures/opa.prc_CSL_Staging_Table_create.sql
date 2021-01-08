IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Staging_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CSL_Staging_Table_create] AS ')
GO
ALTER PROCEDURE [opa].[prc_CSL_Staging_Table_create]
AS
BEGIN


IF OBJECT_ID(N'opa.Staging_Forum', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Forum(
	id INT NOT NULL,
	name NVARCHAR(1000) NULL,
	group_id INT NULL,
	parent__id INT NULL,
	owner__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_ForumThread', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_ForumThread(
	id INT NOT NULL,
	forum__id INT NULL,
	category__id INT NULL,
	owner__id INT NULL,
	title NTEXT NULL,
	description	NTEXT NULL,
	status VARCHAR(1000) NULL,
	last_post__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END


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

IF OBJECT_ID(N'opa.Staging_ForumThreadCategory', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_ForumThreadCategory(
	id INT NOT NULL,
	name NVARCHAR(1000) NULL,
	forum__id INT NULL,
	ctime DATETIME NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_Aggregate', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Aggregate(
	id INT NOT NULL,
	label nvarchar(1000) NULL,
	type varchar(1000) NULL,
	about ntext NULL,
	keywords nvarchar(1000) NULL,
	intendedfor varchar(1000) NULL,
	parent__id Int NULL,
	acs varchar(1000) NULL,
	status varchar(1000) NULL,
	creator__id	Int NULL,
	owner__id Int NULL,
	ctime datetime	 NULL,
	PRIMARY KEY (id)
	)
END




IF OBJECT_ID(N'opa.Staging_CourseAndGroupParticipants', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_CourseAndGroupParticipants(
	id INT NOT NULL,
	user_id	Int NULL,
	aggregate_id Int NULL,
	role varchar(1000) NULL,
	status varchar(1000) NULL,
	subscribe varchar(1000) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_WallPost', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_WallPost(
	id INT NOT NULL,
	group_id Int NULL,
	title nvarchar(1000) NULL,
	isHL int NULL,
	content	ntext NULL,
	type varchar(1000) NULL,
	url	nvarchar(1000) NULL,
	res_slot nvarchar(1000) NULL,
	res_label nvarchar(1000) NULL,
	owner__id Int NULL,
	ctime datetime NULL,
	filename varchar(1000) null,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_Blog', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_Blog(
	id INT NOT NULL,
	title nvarchar(1000) NULL,
	description	ntext NULL,
	owner__id Int NULL,
	group__id Int NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_BlogEntry', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_BlogEntry(
	id INT NOT NULL,
	blog__id varchar(1000) NULL,
	title nvarchar(1000) NULL,
	category nvarchar(1000) NULL,
	content  ntext NULL,
	owner__id Int NULL,
	stime datetime NULL,
	status varchar(1000) NULL,
	res_slot nvarchar(1000) NULL,
	res_file nvarchar(1000) NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Staging_BlogComments', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Staging_BlogComments(
	id INT NOT NULL,
	entry__id varchar(1000) NULL,
	comment ntext NULL,
	owner__id Int NULL,
	ctime datetime NULL,
	PRIMARY KEY (id)
	)
END

END
