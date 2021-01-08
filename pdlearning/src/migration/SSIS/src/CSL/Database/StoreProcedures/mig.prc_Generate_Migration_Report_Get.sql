

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_Generate_Migration_Report_Get' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_Generate_Migration_Report_Get] AS ')
GO
ALTER PROCEDURE [mig].[prc_Generate_Migration_Report_Get]
AS
BEGIN

DECLARE @TimeTaken BIGINT;

SELECT @TimeTaken = SUM(ISNULL(TimeTaken,0)) FROM mig.GenerateMigrationReport WHERE Type = 'Package';

SELECT @TimeTaken 'Time taken to complete running all migration scripts';


SELECT	Folder 'Folder/Database',  Type, Name 'File Name/Table Name', ISNULL(TotalRecords,0) 'Total Records',ISNULL(TotalRecordsAfterMig,0)'Number of records migrated',
ISNULL(TotalRecords,0) - ISNULL(TotalRecordsAfterMig,0) 'Number of records not migrated'
FROM mig.GenerateMigrationReport
WHERE Type IN ('CSV')
ORDER BY Type, Folder, Name;


SELECT	Folder 'Folder/Database',  Type, Name 'File Name/Table Name', ISNULL(TotalRecords,0) 'Total Records',ISNULL(TotalRecordsAfterMig,0)'Number of records after migrated'
FROM mig.GenerateMigrationReport
WHERE Type IN ('DB Table') 
ORDER BY Type, Folder, Name;

END
