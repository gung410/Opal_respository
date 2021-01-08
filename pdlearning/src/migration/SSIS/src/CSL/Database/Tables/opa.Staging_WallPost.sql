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