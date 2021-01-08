BEGIN TRANSACTION [MigrateLearningTrackingData]

BEGIN TRY
    /* 1. Migrate data from Learner DB to Digital Content DB */
    /****** [Learning Tracking]  ******/
    INSERT INTO [ContentDb].[dbo].[LearningTrackings]
      ([Id], [CreatedDate], [ChangedDate], [ItemId], [TrackingType], [TrackingAction], [TotalCount])
    SELECT NEWID() AS Id, [CreatedDate], [ChangedDate], [ItemId], [TrackingType], [TrackingAction], [TotalCount]
    FROM [LearnerDb].[dbo].[LearningTrackings]

COMMIT TRANSACTION [MigrateLearningTrackingData]

END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateLearningTrackingData]
END CATCH  

