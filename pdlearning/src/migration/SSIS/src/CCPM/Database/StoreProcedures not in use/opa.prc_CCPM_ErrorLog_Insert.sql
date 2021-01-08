
ALTER PROCEDURE opa.prc_CCPM_ErrorLog_Insert
AS
BEGIN
	INSERT INTO mig.ErrorLog (MachineName,PackageName,TaskName,FileName,ErrorCode,ErrorDescription,ErrorColumn,StartDate,Created,ErrorRecordId,ErrorRecordData)
	SELECT	DISTINCT t1.computer MachineName ,t1.source PackageName,t2.source TaskName,RIGHT(t2.source,CHARINDEX('_', REVERSE(t2.source)) -1) FileName, t1.datacode ErrorCode, t1.message ErrorDescription, 
	'' ErrorColumn, NULL StartDate, t1.starttime Created, '' ErrorRecordId,'' ErrorRecordData
	FROM dbo.sysssislog t1
	JOIN dbo.sysssislog  t2 ON t1.executionid = t2.executionid AND t1.event = t2.event
	WHERE t1.event = 'OnError' 
	AND t1.source LIKE 'CCPM_%' 
	AND t2.source NOT LIKE 'CCPM_%' 
	
END

