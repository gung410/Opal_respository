BEGIN TRY
	BEGIN TRANSACTION [MigrateAssignment];

	INSERT INTO [LearnerDb].[dbo].[Assignments] ([Id]
      ,[CourseId]
      ,[ClassRunId]
      ,[Title]
      ,[Type]
      ,[CreatedBy]
      ,[ChangedBy]
      ,[CreatedDate]
      ,[ChangedDate])
	  SELECT [Id]
		  ,[CourseId]
		  ,[ClassRunId]
		  ,[Title]
		  ,[Type]
		  ,[CreatedBy]
		  ,[ChangedBy]
		  ,[CreatedDate]
		  ,[ChangedDate] 
		  FROM [CourseDb].[dbo].[Assignment]
		  WHERE IsDeleted = 0

	COMMIT TRANSACTION [MigrateAssignment];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [MigrateAssignment];
END CATCH;
