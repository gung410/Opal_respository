IF OBJECT_ID(N'opa.Migration_FormAnswers', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_FormAnswers(
	Id uniqueidentifier NOT NULL,
	CreatedDate datetime2(7) NOT NULL,
	ChangedDate datetime2(7) NULL,
	ExternalId varchar(255) NULL,
	CreatedBy uniqueidentifier NOT NULL,
	ChangedBy uniqueidentifier NULL,
	FormId uniqueidentifier NOT NULL,
	StartDate datetime2(7) NOT NULL,
	EndDate DATETIME2(7) NULL,
	SubmitDate datetime2(7) NULL,
	Score float NULL,
	ScorePercentage float NULL,
	Attempt smallint NOT NULL,
	FormMetaData nvarchar(max) NULL,
	OwnerId uniqueidentifier NOT NULL,
	IsDeleted bit NOT NULL,
	IsCompleted bit NOT NULL,
	PRIMARY KEY (Id )
)
END