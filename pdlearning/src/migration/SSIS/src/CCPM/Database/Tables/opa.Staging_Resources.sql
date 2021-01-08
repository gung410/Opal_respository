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