-- Migrate data for UserBookmarks

BEGIN TRY
    BEGIN TRANSACTION [MigrateUserBookmarks];
    DECLARE @microLearningTagId VARCHAR(50)= 'db13d0f8-d595-11e9-baec-0242ac120004';
    DECLARE @itemType VARCHAR(30)= 'Course';
    UPDATE [MOE_Learner].[dbo].UserBookmarks
      SET 
          ItemType = 'Microlearning'
    WHERE ItemType = @itemType
          AND ItemId IN
    (
        SELECT c.Id
        FROM [MOE_Course].[dbo].Course c
        WHERE c.PDActivityType = @microLearningTagId
              AND c.Id IN
        (
            SELECT ub.ItemId
            FROM [MOE_Learner].[dbo].UserBookmarks ub
            WHERE ub.ItemType = @itemType
        )
    );
    COMMIT TRANSACTION [MigrateUserBookmarks];
END TRY
BEGIN CATCH
    PRINT('There was an error.');
    ROLLBACK TRANSACTION [MigrateUserBookmarks];
END CATCH;
