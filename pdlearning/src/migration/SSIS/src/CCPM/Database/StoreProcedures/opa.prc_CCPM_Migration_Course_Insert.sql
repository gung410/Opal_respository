IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CCPM_Migration_Course_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'opa')
	EXEC ('CREATE PROC [opa].[prc_CCPM_Migration_Course_Insert] AS ')
GO
ALTER PROCEDURE opa.prc_CCPM_Migration_Course_Insert
AS
BEGIN
	INSERT INTO opa.Migration_Course (	Id,CreatedDate,ChangedDate,DeletedDate,Version,CourseCode,CourseName,CourseType,CourseContent,CourseLevel,Description,
										CourseObjective,DurationMinutes,Price,ThumbnailUrl,TrailerVideoUrl,TermsOfUse,CourseSourceId,CourseCollectionId,
										CopyRightId,IsExternalCourse,ExternalCourseId,ExternalSourceId,CourseCompletionStatus,Status,IsDeleted,Priority,
										ApprovingOfficer,ApprovingOfficerComment,IsAutoPublish,IsAllowDownload,ExternalId,StartDate,EndDate,EndOfRegistration,
										PublishDate,CreatedBy,ChangedBy,ParentId,ParentType,[Order],ExpiredDate,Copyright,Publisher,Source,AcknowledgementAndCredit,
										IsAllowModification,IsAllowReusable,LicenseTerritory,LicenseType,Ownership,Remarks,AttributionUrl)

	SELECT NEWID() Id, CAST(t1.ctime AS DATETIME2(7)) CreatedDate, CAST(t1.ctime AS DATETIME2(7)) ChangedDate, NULL DeletedDate, NULL Version, t1.course_code CourseCode, t1.coursetitle CourseName,
	t1.traisi_course_type CourseType, NULL CourseContent, NULL CourseLevel, t1.description Description, t1.objective CourseObjective, t1.duration_minutes DurationMinutes, NULL Price, 
	NULL ThumbnailUrl, NULL TrailerVideoUrl, NULL TermsOfUse, NULL CourseSourceId, NULL CourseCollectionId, NULL CopyRightId, '0' IsExternalCourse, NULL ExternalCourseId, NULL ExternalSourceId, 
	'0' CourseCompletionStatus, t1.status Status, '0' IsDeleted, NULL Priority, NULL ApprovingOfficer, NULL ApprovingOfficerComment, '0' IsAutoPublish, '0' IsAllowDownload, t1.ID ExternalId, 
	t1.sdate StartDate, t1.edate EndDate, t1. end_of_reg EndOfRegistration, t1.publish_time PublishDate, t3.ExtID CreatedBy, t3.ExtID ChangedBy, NULL ParentId, '0' ParentType, '0' [Order], 
	NULL ExpiredDate, NULL Copyright, t4.ExtID Publisher, t1.source Source,NULL AcknowledgementAndCredit, '0' IsAllowModification, '0' IsAllowReusable, 'Singapore' LicenseTerritory, 
	'Perpetual' LicenseType, 'MoeOwned' Ownership, NULL Remarks, NULL AttributionUrl
	FROM opa.Staging_CourseDetail t1
	LEFT JOIN opa.Migration_Course t2 ON t1.Id = t2.ExternalId
	JOIN opa.Staging_User t3 ON t1.creator__id = t3.ID
	LEFT JOIN opa.Staging_User t4 ON t1.publisher__id = t4.ID
	WHERE t2.ExternalId IS NULL
END
