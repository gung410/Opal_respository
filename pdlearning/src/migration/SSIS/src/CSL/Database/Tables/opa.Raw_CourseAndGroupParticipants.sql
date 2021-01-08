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