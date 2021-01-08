BEGIN TRANSACTION [MigrateUserData]

BEGIN TRY
    /****** Users  ******/
    INSERT INTO [CalendarDb].[dbo].[Users]
      ([Id], [UserID], [FirstName], [LastName], [Email], [EntityStatusID])
    SELECT ExtID, UserID, FirstName, LastName, Email, EntityStatusID
    FROM [db_sam_local].[org].[User]
    WHERE [ExtID]
	      IN	(SELECT DISTINCT [ExtID]
    FROM [db_sam_local].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

COMMIT TRANSACTION [MigrateUserData]

END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateUserData]
END CATCH

