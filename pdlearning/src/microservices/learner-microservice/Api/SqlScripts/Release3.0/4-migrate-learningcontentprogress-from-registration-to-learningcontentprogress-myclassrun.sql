BEGIN TRY
	BEGIN TRANSACTION [MigrateLearningContentProgress];

	UPDATE MyClassRun
	  SET LearningContentProgress = r.LearningContentProgress
	FROM [CourseDb].[dbo].[Registration] r
		 INNER JOIN
		 MyClassRun mcl
		 ON r.Id = mcl.RegistrationId;

	COMMIT TRANSACTION [MigrateLearningContentProgress];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [MigrateLearningContentProgress];
END CATCH;