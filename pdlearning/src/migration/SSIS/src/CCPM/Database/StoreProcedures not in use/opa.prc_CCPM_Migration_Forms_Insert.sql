
ALTER PROCEDURE opa.prc_CCPM_Migration_Forms_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Forms(Id,CreatedDate,ChangedDate,ExternalId,CreatedBy,ChangedBy,LectureId,CourseId,SectionId,Title,Type,Status,
									OwnerId,IsDeleted,DueDate,InSecondTimeLimit,RandomizedQuestions,MaxAttempt,ShowQuizSummary)
	SELECT NEWID() Id,t1.ctime CreatedDate, NULL ChangedDate,t1.ID ExternalId,t3.ExtID CreatedBy,NULL ChangedBy,NULL LectureId,NULL CourseId,NULL SectionId,t1.title Title,t1.type Type,NULL Status,
		   t4.ExtID OwnerId,0 IsDeleted,NULL DueDate,NULL InSecondTimeLimit,t1.rand_qns RandomizedQuestions,NULL MaxAttempt,t1.show_feedback ShowQuizSummary
	FROM opa.Staging_quiz t1
	LEFT JOIN opa.Migration_Forms t2 ON t1.ID = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	JOIN opa.Staging_User t4 ON t1.owner__id = t4.ID
	WHERE t2.ExternalId IS NULL
	AND (t1.level = '')
END
GO
