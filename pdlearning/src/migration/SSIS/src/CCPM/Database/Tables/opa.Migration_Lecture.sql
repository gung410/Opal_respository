IF OBJECT_ID(N'opa.Migration_Lecture', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_Lecture(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ChangedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[SectionId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[Version] [varchar](10) NULL,
	[CopyRightId] [uniqueidentifier] NULL,
	[LectureName] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [varchar](50) NULL,
	[ThumbnailUrl] [varchar](300) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Priority] [int] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[ChangedBy] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[ParentType] [int] NOT NULL,
	[Order] [int] NULL,
	[LectureIcon] [nvarchar](20) NULL,
	[Status] [int] NOT NULL,
	[ExternalId] [varchar](255) NULL,
	PRIMARY KEY(Id)
	)

END