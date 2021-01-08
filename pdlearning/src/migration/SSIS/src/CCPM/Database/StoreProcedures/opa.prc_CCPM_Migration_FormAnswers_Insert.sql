IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_FormAnswers_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_FormAnswers_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_FormAnswers_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormAnswers (	Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,FormId,StartDate,SubmitDate,Score,
											ScorePercentage,Attempt,FormMetaData,OwnerId,IsDeleted,IsCompleted,EndDate)


	SELECT	NEWID() Id, CAST(t1.attempt_time AS DATETIME2(7)) CreatedDate, CAST(t1.attempt_time AS DATETIME2(7)) ChangedDate,  t1.id ExternalId, t3.ExtID CreatedBy, t3.ExtID ChangedBy,
			t5.Id FormId, CAST(t1.attempt_time AS DATETIME2(7)) StartDate, DATEADD(SECOND,duration, attempt_time) SubmitDate, t1.mark Score,
			t1.score ScorePercentage, t1.seq Attempt, NULL FormMetaData, t3.ExtID OwnerId, '0' IsDeleted,
			CASE
				WHEN t1.attempt_complete = 'yes' THEN 1
				ELSE 0
			END AS IsCompleted,
			NULL EndDate
	FROM opa.Staging_userquizattempts t1
	LEFT JOIN opa.Migration_FormAnswers t2 ON t1.Id = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.user__id = t3.ID
	JOIN opa.Migration_Forms t5 ON t5.ExternalId = t1.equiz__id
	WHERE t2.ExternalId IS NULL
END
