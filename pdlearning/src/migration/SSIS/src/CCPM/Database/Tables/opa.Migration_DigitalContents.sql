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