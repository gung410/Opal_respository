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