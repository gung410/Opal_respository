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