-- Update display status for my course

BEGIN TRY
	BEGIN TRANSACTION [UpdateDisplayStatus];
	UPDATE MyCourses
	  SET DisplayStatus = CASE
						  WHEN mcl.RegistrationType = 'Manual' THEN mc.DisplayStatus
						  WHEN mcl.RegistrationType = 'Nominated' THEN 'Nominated' + mc.DisplayStatus
						  WHEN mcl.RegistrationType = 'AddedByCA' THEN 'AddedByCA' + mc.DisplayStatus
						  END

	FROM MyCourses AS mc
		 INNER JOIN
		 MyClassRun AS mcl
		 ON mc.RegistrationId = mcl.RegistrationId
	WHERE mc.CourseType = 'FaceToFace' 
	AND mc.RegistrationId IS NOT NULL
	AND mc.DisplayStatus NOT IN (
		'WithdrawalPendingConfirmation'
		,'WithdrawalRejected'
		,'WithdrawalApproved'
		,'WithdrawalWithdrawn'
		,'WithdrawalRejectedByCA'
		,'ClassRunChangePendingConfirmation'
		,'ClassRunChangeRejected'
		,'ClassRunChangeApproved'
		,'ClassRunChangeConfirmedByCA'
		,'ClassRunChangeRejectedByCA')
	AND (mcl.RegistrationType = 'Nominated' OR mcl.RegistrationType = 'AddedByCA')
	COMMIT TRANSACTION [UpdateDisplayStatus];
END TRY
BEGIN CATCH
	PRINT( 'There was an error.' );
	ROLLBACK TRANSACTION [UpdateDisplayStatus];
END CATCH;
