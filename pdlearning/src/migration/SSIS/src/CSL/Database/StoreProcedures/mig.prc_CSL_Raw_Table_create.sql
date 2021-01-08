IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Raw_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CSL_Raw_Table_create] AS ')
GO
ALTER PROCEDURE [mig].[prc_CSL_Raw_Table_create]
AS
BEGIN


IF OBJECT_ID(N'opa.Raw_Forum', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Forum(
	id INT NOT NULL,
	name NVARCHAR(MAX) NULL,
	group_id VARCHAR(MAX) NULL,
	parent__id VARCHAR(MAX) NULL,
	owner__id VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_ForumThread', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_ForumThread(
	id INT NOT NULL,
	forum__id VARCHAR(MAX) NULL,
	category__id VARCHAR(MAX) NULL,
	owner__id VARCHAR(MAX) NULL,
	title NTEXT NULL,
	description	NTEXT NULL,
	status VARCHAR(MAX) NULL,
	last_post__id VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_ForumThreadCategory', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_ForumThreadCategory(
	id INT NOT NULL,
	name NVARCHAR(MAX) NULL,
	forum__id VARCHAR(MAX) NULL,
	ctime VARCHAR(MAX) NULL,
	PRIMARY KEY (id)
	)
END

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



IF OBJECT_ID(N'opa.Raw_Aggregate', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Aggregate(
	id INT NOT NULL,
	label nvarchar(max) NULL,
	type varchar(max) NULL,
	about ntext NULL,
	keywords nvarchar(max) NULL,
	intendedfor varchar(max) NULL,
	parent__id varchar(max) NULL,
	acs varchar(max) NULL,
	status varchar(max) NULL,
	creator__id	varchar(max) NULL,
	owner__id varchar(max) NULL,
	ctime varchar(max)	 NULL,
	PRIMARY KEY (id)
	)
END




IF OBJECT_ID(N'opa.Raw_CourseAndGroupParticipants', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_CourseAndGroupParticipants(
	id INT NOT NULL,
	user_id	varchar(max) NULL,
	aggregate_id varchar(max) NULL,
	role varchar(max) NULL,
	status varchar(max) NULL,
	subscribe varchar(max) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_WallPost', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_WallPost(
	id INT NOT NULL,
	group_id varchar(max) NULL,
	title nvarchar(max) NULL,
	isHL varchar(max) NULL,
	content	ntext NULL,
	type varchar(max) NULL,
	url	nvarchar(max) NULL,
	res_slot nvarchar(max) NULL,
	res_label nvarchar(max) NULL,
	owner__id varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_Blog', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_Blog(
	id INT NOT NULL,
	title nvarchar(max) NULL,
	description	ntext NULL,
	owner__id varchar(max) NULL,
	group__id varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_BlogEntry', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_BlogEntry(
	id INT NOT NULL,
	blog__id varchar(max) NULL,
	title nvarchar(max) NULL,
	category nvarchar(max) NULL,
	content  ntext NULL,
	owner__id varchar(max) NULL,
	stime varchar(max) NULL,
	status varchar(max) NULL,
	res_slot nvarchar(max) NULL,
	res_file nvarchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END

IF OBJECT_ID(N'opa.Raw_BlogComments', N'U') IS NULL
BEGIN
	CREATE TABLE opa.Raw_BlogComments(
	id INT NOT NULL,
	entry__id varchar(max) NULL,
	comment ntext NULL,
	owner__id varchar(max) NULL,
	ctime varchar(max) NULL,
	PRIMARY KEY (id)
	)
END


END
