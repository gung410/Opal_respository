-- Update status to Incomplete
UPDATE [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack]
SET Status = 'Incomplete'
WHERE Id IN (
	SELECT pat.Id
	FROM [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack] pat JOIN [development-courses-opal-db].[dbo].[Assignment] a on pat.AssignmentId = a.Id
	WHERE pat.SubmittedDate IS NULL AND DATEADD(Day, 30, a.EndDate) < GETDATE()
)

-- Update status to IncompletePendingSubmission
UPDATE [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack]
SET Status = 'IncompletePendingSubmission'
WHERE Id IN (
	SELECT pat.Id
	FROM [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack] pat JOIN [development-courses-opal-db].[dbo].[Assignment] a on pat.AssignmentId = a.Id
	WHERE pat.SubmittedDate IS NULL AND EndDate < GETDATE() AND GETDATE() <= DATEADD(Day, 30, a.EndDate)
)

-- Update status to Completed
UPDATE [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack]
SET Status = 'Completed'
WHERE Id IN (
	SELECT pat.Id
	FROM [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack] pat JOIN [development-courses-opal-db].[dbo].[Assignment] a on pat.AssignmentId = a.Id
	WHERE pat.SubmittedDate IS NOT NULL AND pat.SubmittedDate <= a.EndDate
)

-- Update status to LateSubmission
UPDATE [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack]
SET Status = 'LateSubmission'
WHERE Id IN (
	SELECT pat.Id
	FROM [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack] pat JOIN [development-courses-opal-db].[dbo].[Assignment] a on pat.AssignmentId = a.Id
	WHERE pat.SubmittedDate IS NOT NULL AND pat.SubmittedDate > a.EndDate
)


-- Update status to InProgress
UPDATE [development-courses-opal-db].[dbo].[ParticipantAssignmentTrack]
SET Status = 'InProgress'
WHERE Id IN (
	SELECT ma.ParticipantAssignmentTrackId
	FROM [development-learner-opal-db].[dbo].[MyAssignments] ma
	WHERE ma.Status = 'InProgress'
)
