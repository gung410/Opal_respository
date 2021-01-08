BEGIN TRY
	BEGIN TRANSACTION [MigrateParticipantTrack];
	UPDATE [LearnerDb].[dbo].[MyAssignments]
	  SET StartDate = pt.StartDate, EndDate = pt.EndDate
	FROM [CourseDb].[dbo].[ParticipantAssignmentTrack] pt
		 INNER JOIN
		 [LearnerDb].[dbo].[MyAssignments] ma
		 ON pt.Id = ma.Id;

	COMMIT TRANSACTION [MigrateParticipantTrack];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [MigrateParticipantTrack];
END CATCH;
