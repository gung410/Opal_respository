-- Migrate RegistrationId from Registration in Course to MyClassRun in Learner

BEGIN TRY
	BEGIN TRANSACTION [UpdateRegistrationId];

	UPDATE MyCourses
	  SET RegistrationId = mclOuter.RegistrationId
	FROM MyCourses mcOuter
		 INNER JOIN
	(
		SELECT mc.UserId, mc.CourseId, MAX(CreatedDate) AS maxCreationDate
		FROM MyCourses AS mc
		WHERE CourseType = 'FaceToFace' AND 
			  DisplayStatus IS NOT NULL AND 
			  MyRegistrationStatus IS NOT NULL AND
			  IsDeleted = 0
		GROUP BY mc.UserId, mc.CourseId
	) mcInner
		 ON mcOuter.UserId = mcInner.UserId AND 
			mcOuter.CourseId = mcInner.CourseId AND 
			mcOuter.CreatedDate = mcInner.maxCreationDate
		 INNER JOIN
	(
		SELECT mclOuter.CourseId, mclOuter.UserId, mclOuter.RegistrationId, mclOuter.ClassRunId
		FROM MyClassRun AS mclOuter
			 INNER JOIN
		(
			SELECT cl.UserId, cl.CourseId, MAX(CreatedDate) AS maxCreationDate
			FROM MyClassRun AS cl
			WHERE cl.IsDeleted = 0
			GROUP BY cl.UserId, cl.CourseId
		) AS mclInner
			 ON mclOuter.UserId = mclInner.UserId AND 
				mclOuter.CourseId = mclInner.CourseId AND 
				mclOuter.CreatedDate = mclInner.maxCreationDate
	) mclOuter
		 ON mclOuter.CourseId = mcOuter.CourseId AND 
			mclOuter.UserId = mcOuter.UserId;

	COMMIT TRANSACTION [UpdateRegistrationId];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [UpdateRegistrationId];
END CATCH;
