
CREATE PROCEDURE opa.prc_Humhub_Staging_CourseAndGroupParticipants_Insert
AS
BEGIN
	INSERT INTO opa.Staging_CourseAndGroupParticipants (id,user_id,aggregate_id,role,status,subscribe)
	SELECT t1.id,t1.user_id,t1.aggregate_id,t1.role,t1.status,t1.subscribe
	FROM opa.Raw_CourseAndGroupParticipants t1
	LEFT JOIN opa.Staging_CourseAndGroupParticipantst2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
