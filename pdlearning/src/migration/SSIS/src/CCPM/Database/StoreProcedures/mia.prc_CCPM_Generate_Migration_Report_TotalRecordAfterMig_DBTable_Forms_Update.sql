IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Forms_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Forms_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Forms_Update]
@TotalRecord_AfterMigrated_Forms INT,
@TotalRecord_AfterMigrated_FormQuestions INT,
@TotalRecord_AfterMigrated_FormAnswers INT,
@TotalRecord_AfterMigrated_FormQuestionAnswers INT,
@TotalRecord_AfterMigrated_SharedQuestions INT
AS
BEGIN

	UPDATE mig.GenerateMigrationReport 
	SET TotalRecordsAfterMig =  CASE 
									WHEN Name = 'dbo.Forms' THEN @TotalRecord_AfterMigrated_Forms
									WHEN Name = 'dbo.FormQuestions' THEN @TotalRecord_AfterMigrated_FormQuestions
									WHEN Name = 'dbo.FormAnswers' THEN @TotalRecord_AfterMigrated_FormAnswers
									WHEN Name = 'dbo.FormQuestionAnswers' THEN @TotalRecord_AfterMigrated_FormQuestionAnswers
									WHEN Name = 'dbo.SharedQuestions' THEN @TotalRecord_AfterMigrated_SharedQuestions
									ELSE 0 
								END
	WHERE Folder = 'CCPM' AND Type = 'DBTable' AND Name in ('dbo.Forms', 'dbo.FormQuestions', 'dbo.FormAnswers', 'dbo.FormQuestionAnswers', 'dbo.SharedQuestions')

	UPDATE mig.GenerateMigrationReport 
	SET TotalRecordsAfterMig = CASE 
									WHEN Name = 'quiz' THEN @TotalRecord_AfterMigrated_Forms + @TotalRecord_AfterMigrated_FormQuestions
									WHEN Name = 'user quiz attempts' THEN @TotalRecord_AfterMigrated_FormAnswers
									WHEN Name = 'user question attempts' THEN @TotalRecord_AfterMigrated_SharedQuestions
									ELSE 0 
								END
	WHERE Folder = 'Quiz' AND Type = 'CSV' AND Name in ('quiz' , 'user quiz attempts', 'user question attempts')

	UPDATE mig.GenerateMigrationReport 
	SET EndTime = GETDATE(),
		TimeTaken = DATEDIFF(SECOND, StartTime, GETDATE())
	WHERE Folder = 'CCPM' AND Name = 'CCPM_3.3_TransferProductionForms' AND Type = 'Package'


END