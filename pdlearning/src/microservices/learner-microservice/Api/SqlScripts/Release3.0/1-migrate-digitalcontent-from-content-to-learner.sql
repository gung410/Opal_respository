BEGIN TRY
    BEGIN TRANSACTION [MigrateDigitalContent];

	INSERT INTO [LearnerDb].[dbo].[DigitalContents]( [Id], [CreatedDate], [ChangedDate], [Title], [Description], [Type], [Status], [OriginalObjectId], [OwnerId],[FileExtension] )
		   SELECT Id, [CreatedDate], [ChangedDate], [Title], [Description], [Type], [Status], [OriginalObjectId], [OwnerId],[FileExtension]
		   FROM [ContentDb].[dbo].[DigitalContents];

     COMMIT TRANSACTION [MigrateDigitalContent];
END TRY
BEGIN CATCH
    PRINT('There was an error.');
    ROLLBACK TRANSACTION [MigrateDigitalContent];
END CATCH;
