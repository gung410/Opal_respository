IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_Table_create' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_Table_create] AS ')
GO
ALTER PROCEDURE [opa].[prc_CCPM_Migration_Table_create]
AS
BEGIN

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

IF OBJECT_ID(N'opa.Migration_FormQuestions', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_FormQuestions(
	Id uniqueidentifier NOT NULL,
	CreatedDate datetime2(7) NOT NULL,
	ChangedDate datetime2(7) NULL,
	ExternalId varchar(255) NULL,
	CreatedBy uniqueidentifier NOT NULL,
	ChangedBy uniqueidentifier NULL,
	Question_Type varchar(30) NOT NULL,
	Question_Title nvarchar(3000) NULL,
	Question_CorrectAnswer nvarchar(max) NULL,
	Question_Options nvarchar(max) NULL,
	Question_Hint nvarchar(max) NULL,
	Question_AnswerExplanatoryNote nvarchar(max) NULL,
	Question_Level int NULL,
	FormId uniqueidentifier NOT NULL,
	Title nvarchar(3000) NULL,
	Priority int NOT NULL,
	ShowFeedBackAfterAnswer bit NULL,
	RandomizedOptions bit NULL,
	Score float NULL,
	IsDeleted bit NOT NULL,
	PRIMARY KEY (Id )
	)
END 

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

IF OBJECT_ID(N'opa.Migration_FormQuestionAnswers', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_FormQuestionAnswers(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ChangedDate] [datetime2](7) NULL,
	[ExternalId] [varchar](255) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[ChangedBy] [uniqueidentifier] NULL,
	[FormAnswerId] [uniqueidentifier] NOT NULL,
	[FormQuestionId] [uniqueidentifier] NOT NULL,
	[AnswerValue] [nvarchar](max) NULL,
	[MaxScore] [float] NULL,
	[Score] [float] NULL,
	[ScoredBy] [uniqueidentifier] NULL,
	[AnswerFeedback] [nvarchar](max) NULL,
	[SubmittedDate] [datetime2](7) NULL,
	[SpentTimeInSeconds] [int] NULL,
	PRIMARY KEY (Id )
)
END 

IF OBJECT_ID(N'opa.Migration_SharedQuestions', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_SharedQuestions(
	Id uniqueidentifier NOT NULL,
	CreatedDate datetime2(7) NOT NULL,
	ChangedDate datetime2(7) NULL,
	ExternalId varchar(255) NULL,
	CreatedBy uniqueidentifier NOT NULL,
	ChangedBy uniqueidentifier NULL,
	Question_Type varchar(30) NOT NULL,
	Question_Title ntext NULL,
	Question_CorrectAnswer nvarchar(max) NULL,
	Question_Options nvarchar(max) NULL,
	Question_Hint nvarchar(max) NULL,
	Question_AnswerExplanatoryNote nvarchar(max) NULL,
	Question_Level int NULL,
	OwnerId uniqueidentifier NOT NULL,
	IsDeleted bit NOT NULL,
	PRIMARY KEY (Id )
)
END


IF OBJECT_ID(N'opa.Migration_DigitalContents', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_DigitalContents(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ChangedDate] [datetime2](7) NULL,
	[Title] [nvarchar](255) NULL,
	[Description] ntext NULL,
	[Type] [varchar](20) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ChangedBy] [uniqueidentifier] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[OwnerId] [uniqueidentifier] NOT NULL,
	[ExternalId] [varchar](255) NULL,
	[RepositoryName] [nvarchar](100) NULL,
	[Discriminator] [nvarchar](max) NOT NULL,
	[HtmlContent] [nvarchar](max) NULL,
	[FileName] [nvarchar](255) NULL,
	[FileType] [varchar](100) NULL,
	[FileExtension] [varchar](10) NULL,
	[FileSize] [float] NULL,
	[FileLocation] [nvarchar](255) NULL,
	[ExpiredDate] [datetime2](7) NULL,
	[Copyright] [nvarchar](100) NULL,
	[Publisher] [nvarchar](100) NULL,
	[Source] [nvarchar](255) NULL,
	[TermsOfUse] [nvarchar](4000) NULL,
	PRIMARY KEY(Id)
	)
END
END
