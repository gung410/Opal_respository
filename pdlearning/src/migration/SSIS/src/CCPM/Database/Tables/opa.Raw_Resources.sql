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