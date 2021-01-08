-- apply for development and uat-next environment
-- Step 1: Create UserTemporary if none exists (on SQL server)
CREATE TABLE [dbo].[UserTemporary](
	[Id] [uniqueidentifier] NOT NULL,
	[OriginalUserId] [int] NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Email] [nvarchar](256) NULL,
	[ExtId] [uniqueidentifier] NOT NULL,
	[AvatarUrl] [nvarchar](max) NULL,
	[Followers] [uniqueidentifier] NULL,
 CONSTRAINT [PK_UserTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
-- Step 2: insert into [UserTemporary]
INSERT INTO [dbo].[UserTemporary]( 
			[Id], 
			[OriginalUserId], 
			[FirstName], 
			[LastName], 
			[Email], 
			[ExtId], 
			[AvatarUrl] )
SELECT      NEWID(),
			u.UserID, 
			u.FirstName, 
			u.LastName, 
			u.Email,	
			[ExtID], 
			JSON_VALUE(u.DynamicAttributes, '$.avatarUrl')
	   FROM [development-competence-opal-at6qr].[org].[User] AS u
	   WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL

-- Step 3: Migrate from UserTemporary on SQL to UserTemporary collection mongodb by Studio 3T tool.
-- Step 4: Trigger api api/newsFeed/migrateUserTempToUser to migrate from UserTemporary to User on mongodb.
-- Because Studio 3t doesn't support to migrate for Guid type
