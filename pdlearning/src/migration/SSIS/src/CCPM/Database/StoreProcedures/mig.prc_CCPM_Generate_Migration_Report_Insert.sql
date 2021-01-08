IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_Insert] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_Insert]
AS
BEGIN
	INSERT INTO mig.GenerateMigrationReport(Folder, Name, Type, TotalRecords, StartTime, EndTime, TimeTaken)
	VALUES('Repository','HR, Finance, Ops & Admin','CSV',NULL,NULL,NULL,NULL),
	('Repository','Leadership Policies','CSV',NULL,NULL,NULL,NULL),
	('Repository','Partners and Engagement','CSV',NULL,NULL,NULL,NULL),
	('Repository','Professional Development','CSV',NULL,NULL,NULL,NULL),
	('Repository','Publications','CSV',NULL,NULL,NULL,NULL),
	('Repository','Research','CSV',NULL,NULL,NULL,NULL),
	('CCPM','dbo.DigitalContents','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','CCPM_1.1_ImportRepositories','Package',NULL,GETDATE(),NULL,NULL),
	('Quiz','quiz','CSV',NULL,NULL,NULL,NULL),
	('Quiz','user quiz attempts','CSV',NULL,NULL,NULL,NULL),
	('Quiz','user question attempts','CSV',NULL,NULL,NULL,NULL),
	('CCPM','dbo.Forum','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.ForumPost','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.ForumThread','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.ForumThreadCategory','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','CCPM_2.1_ImportQuiz','Package',NULL,GETDATE(),NULL,NULL),
	('CCPM','dbo.Forms','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.FormQuestions','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.FormAnswers','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.FormQuestionAnswers','DB Table',NULL,NULL,NULL,NULL),
	('CCPM','dbo.SharedQuestions','DB Table',NULL,NULL,NULL,NULL)
END