
ALTER PROCEDURE opa.prc_CCPM_Migration_FormAnswers_Insert
AS
BEGIN
	INSERT INTO opa.Migration_FormAnswers(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,FormId,StartDate,EndDate,SubmitDate,
										  Score,ScorePercentage,Attempt,FormMetaData,OwnerId,IsDeleted,IsCompleted)
	SELECT NEWID() Id,GETDATE() CreatedDate ,NULL ChangedDate,t1.ID ExternalId,t4.ExtID CreateBy,NULL ChangedBy,t3.Id FormId,t1.attempt_time StartDate, NULL EndDate,DATEADD(SECOND,t1.duration,t1.attempt_time) SubmitDate,
	t1.mark Score,t1.score ScorePercentage,t1.seq Attempt,'NEED DEVELOPER INSERT' FormMetaData, t4.ExtID OwnerId,0 IsDeleted,
	CAST(	CASE
					WHEN t1.attempt_complete = 'yes'
						THEN 1
					ELSE 0
			END AS bit) as IsCompleted
	FROM opa.Staging_userquizattempts t1
	LEFT JOIN opa.Migration_FormAnswers t2 ON t1.ID = t2.ExternalId
	JOIN opa.Migration_Forms t3 ON t1.equiz__id = t3.ExternalId
	JOIN opa.Staging_User t4 ON t1.user__id = t4.ID
	WHERE t2.ExternalId IS NULL
END
GO
