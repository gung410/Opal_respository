-- Migrate data: ClassRun from CAM to ClassRun in LEARNER
BEGIN TRY
		BEGIN TRANSACTION [MigrateClassRunFromCamToLearner]

			-- Set Id = NEWID()
			INSERT INTO [MOE_Learner].[dbo].[ClassRun] ([Id],[ClassRunId],[CourseId],[ClassTitle],[ClassRunCode],[StartDateTime],[EndDateTime],[PlanningStartTime],[PlanningEndTime],
												[FacilitatorIds],[CoFacilitatorIds],[ContentStatus],[PublishedContentDate],[SubmittedContentDate],[ApprovalContentDate],
												[MinClassSize],[MaxClassSize],[ApplicationStartDate],[ApplicationEndDate],[Status],[ClassRunVenueId],[CancellationStatus],
												[RescheduleStatus],[RescheduleStartDateTime],[RescheduleEndDateTime],[Reason],[CreatedBy],[ChangedBy],[ExternalId],
												[CreatedDate],[ChangedDate])
												SELECT NEWID() as Id,[Id] as [ClassRunId],[CourseId],[ClassTitle],[ClassRunCode],[StartDateTime],[EndDateTime],[PlanningStartTime],[PlanningEndTime],
														[FacilitatorIds],[CoFacilitatorIds],[ContentStatus],[PublishedContentDate],[SubmittedContentDate],[ApprovalContentDate],
														[MinClassSize],[MaxClassSize],[ApplicationStartDate],[ApplicationEndDate],[Status],[ClassRunVenueId],[CancellationStatus],
														[RescheduleStatus],[RescheduleStartDateTime],[RescheduleEndDateTime],[Reason],[CreatedBy],[ChangedBy],[ExternalId],
														[CreatedDate],[ChangedDate] 
														FROM [MOE_Course].[dbo].[ClassRun]

	COMMIT TRANSACTION [MigrateClassRunFromCamToLearner]
END TRY  
BEGIN CATCH 
	print('There was an error.')
    ROLLBACK TRANSACTION [MigrateClassRunFromCamToLearner]
END CATCH
