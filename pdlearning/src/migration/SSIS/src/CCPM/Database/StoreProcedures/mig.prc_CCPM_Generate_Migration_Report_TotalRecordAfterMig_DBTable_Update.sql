IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CCPM_Generate_Migration_Report_TotalRecordAfterMig_DBTable_Update]
@TotalRecordAfterMig INT,
@TotalRecord_HrAdmin INT,
@TotalRecord_LeaderShip INT,
@TotalRecord_Partner INT,
@TotalRecord_ProDev INT,
@TotalRecord_Publications INT,
@TotalRecord_Research INT
AS
BEGIN

	UPDATE mig.GenerateMigrationReport 
	SET TotalRecordsAfterMig = @TotalRecordAfterMig
	WHERE Folder = 'CCPM' AND Name= 'dbo.DigitalContents' AND Type = 'DB Table'

	UPDATE mig.GenerateMigrationReport 
	SET TotalRecordsAfterMig = CASE 
									WHEN Name = 'HR, Finance, Ops & Admin' THEN @TotalRecord_HrAdmin
									WHEN Name = 'Leadership Policies' THEN @TotalRecord_LeaderShip
									WHEN Name = 'Partners and Engagement' THEN @TotalRecord_Partner
									WHEN Name = 'Professional Development' THEN @TotalRecord_ProDev
									WHEN Name = 'Publications' THEN @TotalRecord_Publications
									WHEN Name = 'Research' THEN @TotalRecord_Research
									ELSE 0 
								END
	WHERE Folder = 'Repository' AND Type = 'CSV'

	UPDATE mig.GenerateMigrationReport 
	SET EndTime = GETDATE(),
		TimeTaken = DATEDIFF(SECOND, StartTime, GETDATE())
	WHERE Folder = 'CCPM' AND Name = 'CCPM_1.3_TransferProductionDigitalContent' AND Type = 'Package'

END