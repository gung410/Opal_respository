-- Migrate post course evaluation from Registration on Course to MyClassRun on Learner

BEGIN TRY
	BEGIN TRANSACTION [UpdatePostCourse];
	UPDATE MyClassRun
	  SET 
	  PostCourseEvaluationFormCompleted = r.PostCourseEvaluationFormCompleted
	FROM MyClassRun cl
		 INNER JOIN
		 [CourseDb].[dbo].[Registration] r
		 ON cl.RegistrationId = r.Id
	where cl.PostCourseEvaluationFormCompleted <> r.PostCourseEvaluationFormCompleted
	COMMIT TRANSACTION [UpdatePostCourse];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [UpdatePostCourse];
END CATCH;
