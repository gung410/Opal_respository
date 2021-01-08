IF OBJECT_ID(N'opa.Migration_Forms', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_Forms(
	Id uniqueidentifier NOT NULL,
	CreatedDate datetime2(7) NOT NULL,
	ChangedDate datetime2(7) NULL,
	ExternalId varchar(255) NULL,
	CreatedBy uniqueidentifier NULL,
	ChangedBy uniqueidentifier NULL,
	LectureId uniqueidentifier NULL,
	CourseId uniqueidentifier NULL,
	SectionId uniqueidentifier NULL,
	Title nvarchar(1000) NULL,
	Type varchar(30) NOT NULL,
	Status varchar(30) NOT NULL,
	OwnerId uniqueidentifier NOT NULL,
	IsDeleted bit NOT NULL,
	DueDate datetime2(7) NULL,
	InSecondTimeLimit int NULL,
	RandomizedQuestions bit NOT NULL,
	MaxAttempt smallint NULL,
	ShowQuizSummary bit NULL,
	PRIMARY KEY (Id )
	)
END