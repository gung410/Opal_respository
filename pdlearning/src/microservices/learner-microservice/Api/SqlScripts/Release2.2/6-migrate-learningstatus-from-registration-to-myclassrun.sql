-- Migrate learning status from Registration in Course to MyClassRun in Learner

BEGIN TRY
	BEGIN TRANSACTION [UpdateLearningStatus];
	UPDATE MyClassRun
	  SET 
	  LearningStatus = r.LearningStatus
	FROM MyClassRun cl
		 INNER JOIN
		 [CourseDb].[dbo].[Registration] r
		 ON cl.RegistrationId = r.Id
	where cl.LearningStatus <> r.LearningStatus
	COMMIT TRANSACTION [UpdateLearningStatus];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [UpdateLearningStatus];
END CATCH;
