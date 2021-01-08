IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Get' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Get] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_DBTable_Get]
@Type VARCHAR(50)
AS
BEGIN

	IF @Type = 'DigitalContent' 
		BEGIN
			DECLARE @TotalRecord INT;
			DECLARE @TotalRecord_HrAdmin INT;
			DECLARE @TotalRecord_LeaderShip INT;
			DECLARE @TotalRecord_Partner INT;
			DECLARE @TotalRecord_ProDev INT;
			DECLARE @TotalRecord_Publications INT;
			DECLARE @TotalRecord_Research INT;

			SELECT @TotalRecord = COUNT(Id)
			FROM dbo.DigitalContents;

			IF OBJECT_ID('tempdb.dbo.#temp_repository', 'U') IS NOT NULL
			DROP TABLE #temp_repository; 
			CREATE TABLE #temp_repository(
			  RepositoryName VARCHAR(100) NOT NULL,
			  TotalRecords INT  NOT NULL,
			PRIMARY KEY (RepositoryName)
			)

			INSERT INTO #temp_repository(RepositoryName,TotalRecords)
			SELECT RepositoryName, COUNT(id) 
			FROM dbo.DigitalContents 
			WHERE RepositoryName IS NOT NULL
			GROUP BY RepositoryName

			SELECT @TotalRecord_HrAdmin = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'HR, Finance, Ops & Admin'

			SELECT @TotalRecord_LeaderShip = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'Leadership and Policies'

			SELECT @TotalRecord_Partner = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'Partners and Engagement'

			SELECT @TotalRecord_ProDev = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'Professional Development'

			SELECT @TotalRecord_Publications = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'Publications'

			SELECT @TotalRecord_Research = TotalRecords
			FROM #temp_repository
			WHERE RepositoryName = 'Research'


			SELECT @TotalRecord TotalRecord, @TotalRecord_HrAdmin TotalRecord_HrAdmin, @TotalRecord_LeaderShip TotalRecord_LeaderShip, @TotalRecord_Partner TotalRecord_Partner, 
			@TotalRecord_ProDev TotalRecord_ProDev, @TotalRecord_Publications TotalRecord_Publications, @TotalRecord_Research TotalRecord_Research
	END

	IF @Type = 'Forms' 
		BEGIN
			DECLARE @TotalRecord_Forms INT;
			DECLARE @TotalRecord_FormQuestions INT;
			DECLARE @TotalRecord_FormAnswers INT;
			DECLARE @TotalRecord_FormQuestionAnswers INT;
			DECLARE @TotalRecord_SharedQuestions INT;

			SELECT @TotalRecord_Forms = COUNT(1) 
			FROM Forms
			WHERE ExternalId IS NULL

			SELECT @TotalRecord_FormQuestions = COUNT(1) 
			FROM FormQuestions
			WHERE ExternalId IS NULL

			SELECT @TotalRecord_FormAnswers = COUNT(1) 
			FROM FormAnswers
			WHERE ExternalId IS NULL

			SELECT @TotalRecord_FormQuestionAnswers = COUNT(1) 
			FROM FormQuestionAnswers
			WHERE ExternalId IS NULL

			SELECT @TotalRecord_SharedQuestions = COUNT(1) 
			FROM SharedQuestions
			WHERE ExternalId IS NULL

			SELECT @TotalRecord_Forms TotalRecord_Forms, @TotalRecord_FormQuestions TotalRecord_FormQuestions, @TotalRecord_FormAnswers TotalRecord_FormAnswers, 
			@TotalRecord_FormQuestionAnswers TotalRecord_FormQuestionAnswers, @TotalRecord_SharedQuestions TotalRecord_SharedQuestions

		END

	IF @Type = 'Forms_AfterMigrated' 
		BEGIN
			DECLARE @TotalRecord_AfterMigrated_Forms INT;
			DECLARE @TotalRecord_AfterMigrated_FormQuestions INT;
			DECLARE @TotalRecord_AfterMigrated_FormAnswers INT;
			DECLARE @TotalRecord_AfterMigrated_FormQuestionAnswers INT;
			DECLARE @TotalRecord_AfterMigrated_SharedQuestions INT;

			SELECT @TotalRecord_AfterMigrated_Forms = COUNT(1) 
			FROM Forms
			WHERE ExternalId IS NOT NULL

			SELECT @TotalRecord_AfterMigrated_FormQuestions = COUNT(1) 
			FROM FormQuestions
			WHERE ExternalId IS NOT NULL

			SELECT @TotalRecord_AfterMigrated_FormAnswers = COUNT(1) 
			FROM FormAnswers
			WHERE ExternalId IS NOT NULL

			SELECT @TotalRecord_AfterMigrated_FormQuestionAnswers = COUNT(1) 
			FROM FormQuestionAnswers
			WHERE ExternalId IS NOT NULL

			SELECT @TotalRecord_AfterMigrated_SharedQuestions = COUNT(1) 
			FROM SharedQuestions
			WHERE ExternalId IS NOT NULL

			SELECT @TotalRecord_AfterMigrated_Forms TotalRecord_AfterMigrated_Forms, @TotalRecord_AfterMigrated_FormQuestions TotalRecord_AfterMigrated_FormQuestions,
			@TotalRecord_AfterMigrated_FormAnswers TotalRecord_AfterMigrated_FormAnswers, 
			@TotalRecord_AfterMigrated_FormQuestionAnswers TotalRecord_AfterMigrated_FormQuestionAnswers, @TotalRecord_AfterMigrated_SharedQuestions TotalRecord_AfterMigrated_SharedQuestions

		END

END