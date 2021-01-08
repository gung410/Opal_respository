

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_Generate_Migration_Report_ErrorLog_Get' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_Generate_Migration_Report_ErrorLog_Get] AS ')
GO
ALTER PROCEDURE [mig].[prc_Generate_Migration_Report_ErrorLog_Get]
AS
BEGIN

	SELECT COUNT(ID) 'Number of migration errors' FROM mig.ErrorLog;

	SELECT PackageName 'Package Name', TaskName 'Task Name', FileName 'CSV File Name/Table Name', ErrorDescription 'Error Description', Created 'Log Date', ErrorRecordId 'Error Record Id', 
	ErrorColumnName 'Error Column Name'
	FROM mig.ErrorLog

END