BEGIN TRANSACTION [MigrateUserData]

BEGIN TRY
    /* 1. Migrate Users from SAM to Webinar DB */

    INSERT INTO [WebinarDb].[dbo].[Users]
      ([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId], [AvatarUrl])
    SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID, JSON_VALUE(DynamicAttributes, '$.avatarUrl') as AvatarUrl
    FROM [db_sam_local].[org].[User]
    WHERE [ExtID]
	      IN (SELECT DISTINCT [ExtID]
    FROM [db_sam_local].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

COMMIT TRANSACTION [MigrateUserData]

END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateUserData]
END CATCH  

