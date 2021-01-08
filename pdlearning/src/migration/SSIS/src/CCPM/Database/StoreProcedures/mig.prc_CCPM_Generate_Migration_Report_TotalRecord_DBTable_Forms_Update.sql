IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Forms_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Forms_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Forms_Update]
@TotalRecord_Forms INT,
@TotalRecord_FormQuestions INT,
@TotalRecord_FormAnswers INT,
@TotalRecord_FormQuestionAnswers INT,
@TotalRecord_SharedQuestions INT
AS
BEGIN

	UPDATE mig.GenerateMigrationReport 
	SET TotalRecords =  CASE 
							WHEN Name = 'dbo.Forms' THEN @TotalRecord_Forms
							WHEN Name = 'dbo.FormQuestions' THEN @TotalRecord_FormQuestions
							WHEN Name = 'dbo.FormAnswers' THEN @TotalRecord_FormAnswers
							WHEN Name = 'dbo.FormQuestionAnswers' THEN @TotalRecord_FormQuestionAnswers
							WHEN Name = 'dbo.SharedQuestions' THEN @TotalRecord_SharedQuestions
							ELSE 0 
						END
	WHERE Folder = 'CCPM' AND Type = 'DBTable' AND Name in ('dbo.Forms', 'dbo.FormQuestions', 'dbo.FormAnswers', 'dbo.FormQuestionAnswers', 'dbo.SharedQuestions')


END

