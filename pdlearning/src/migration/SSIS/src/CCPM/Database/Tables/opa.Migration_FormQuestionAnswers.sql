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