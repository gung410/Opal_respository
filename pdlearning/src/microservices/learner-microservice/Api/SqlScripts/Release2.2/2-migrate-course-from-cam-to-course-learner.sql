BEGIN TRY
    BEGIN TRANSACTION [MigrateCourse];

   INSERT INTO [LearnerDb].[dbo].[Course]
    ([Id], 
	[CreatedDate], [ChangedDate],[CourseId], [CourseName], 
	[LearningMode], [CourseCode], [Description], [MOEOfficerId], 
	[PDActivityType], [SubmittedDate], [CourseType], [MaxReLearningTimes], 
	[FirstAdministratorId], [SecondAdministratorId], [PrimaryApprovingOfficerId], 
	[AlternativeApprovingOfficerId], [Status], [ContentStatus], 
	[PublishedContentDate], [SubmittedContentDate], [Version], 
	[CreatedBy], [ChangedBy], [ApprovalDate], [ApprovalContentDate], 
	[PublishDate], [ArchiveDate], [Source], [StartDate], [ExpiredDate], [DepartmentId]
    ) 
	SELECT 
		NEWID(), 
		[CreatedDate], [ChangedDate], [Id], [CourseName], 
		[LearningMode], [CourseCode], [Description], [MOEOfficerId], 
		[PDActivityType], [SubmittedDate], [CourseType], [MaxReLearningTimes], 
		[FirstAdministratorId], [SecondAdministratorId], [PrimaryApprovingOfficerId], 
		[AlternativeApprovingOfficerId], [Status], [ContentStatus], 
		[PublishedContentDate], [SubmittedContentDate], [Version], 
		[CreatedBy], [ChangedBy], [ApprovalDate], [ApprovalContentDate], 
		[PublishDate], [ArchiveDate], [Source], [StartDate], [ExpiredDate], [DepartmentId]
		FROM [CourseDb].[dbo].[Course];

     COMMIT TRANSACTION [MigrateCourse];
END TRY
BEGIN CATCH
    PRINT('There was an error.');
    ROLLBACK TRANSACTION [MigrateCourse];
END CATCH;
