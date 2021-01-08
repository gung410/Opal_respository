IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_TotalRecord_CSV_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_CSV_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_TotalRecord_CSV_Update]
AS
BEGIN
	DECLARE @TotalRecord_HrAdmin INT
	DECLARE @TotalRecord_LeaderShip_Policies INT
	DECLARE @TotalRecord_Partners INT
	DECLARE @TotalRecord_ProDev INT
	DECLARE @TotalRecord_Publications INT
	DECLARE @TotalRecord_Research INT
	DECLARE @TotalRecord_quiz INT
	DECLARE @TotalRecord_userquizattempts INT
	DECLARE @TotalRecord_userquestionattempts INT

	SELECT 		
	@TotalRecord_HrAdmin =	SUM(	CASE
										WHEN repository_name = 'HR, Finance, Ops & Admin' THEN 1
										ELSE 0
									END), 
	@TotalRecord_LeaderShip_Policies =	SUM(	CASE
										WHEN repository_name = 'Leadership and Policies' THEN 1
										ELSE 0
									END),
	@TotalRecord_Partners =	SUM(	CASE
										WHEN repository_name = 'Partners and Engagement' THEN 1
										ELSE 0
									END),
	@TotalRecord_ProDev =	SUM(	CASE
										WHEN repository_name = 'Professional Development' THEN 1
										ELSE 0
									END),
	@TotalRecord_Publications =	SUM(	CASE
										WHEN repository_name = 'Publications' THEN 1
										ELSE 0
									END),
	@TotalRecord_Research =	SUM(	CASE
										WHEN repository_name = 'Research' THEN 1
										ELSE 0
									END)
	FROM opa.Raw_Resources;


	SELECT @TotalRecord_quiz = COUNT(ID) FROM opa.Raw_quiz;

	SELECT @TotalRecord_userquizattempts = COUNT(ID) FROM opa.Raw_userquizattempts;

	SELECT @TotalRecord_userquestionattempts = COUNT(ID) FROM opa.Raw_userquestionattempts;


	UPDATE mig.GenerateMigrationReport
	SET TotalRecords = (CASE
						WHEN Name = 'HR, Finance, Ops & Admin' THEN  @TotalRecord_HrAdmin
						WHEN Name = 'Leadership Policies' THEN  @TotalRecord_LeaderShip_Policies
						WHEN Name = 'Partners and Engagement' THEN  @TotalRecord_Partners
						WHEN Name = 'Professional Development' THEN  @TotalRecord_ProDev
						WHEN Name = 'Publications' THEN  @TotalRecord_Publications
						WHEN Name = 'Research' THEN  @TotalRecord_Research
						WHEN Name = 'quiz' THEN  @TotalRecord_quiz
						WHEN Name = 'user quiz attempts' THEN  @TotalRecord_userquizattempts
						WHEN Name = 'user question attempts' THEN  @TotalRecord_userquestionattempts
						ELSE 0 
						END)
	WHERE Type = 'CSV' AND Folder IN('Repository','Quiz')
END