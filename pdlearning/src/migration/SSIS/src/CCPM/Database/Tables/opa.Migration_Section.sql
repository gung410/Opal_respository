IF OBJECT_ID(N'opa.Migration_Section', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_Section(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ChangedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[ParentSectionId] [uniqueidentifier] NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[Version] [varchar](10) NULL,
	[Title] [varchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Priority] [int] NULL,
	[SectionSeqPath] [varchar](128) NULL,
	[Level] [varchar](20) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[ChangedBy] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[ParentType] [int] NOT NULL,
	[Order] [int] NULL,
	[ExternalId] [varchar](255) NULL,
	PRIMARY KEY(Id)
	)
END