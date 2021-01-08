IF (OBJECT_ID('tempdb..#TempTable') IS NOT NULL)
BEGIN
    DROP TABLE #TempTable
END
GO

BEGIN
	BEGIN
		SELECT DISTINCT cUser.Id, uApprovalGroup.ExtID, ugt.ExtId as UserGroupTypeExtId
		INTO #TempTable
		FROM [development-competence-opal-at6qr].[org].[User] u
		JOIN [development-competence-opal-at6qr].[org].[UGMember] ugm ON u.UserID = ugm.UserId AND ugm.EntityStatusID = 1
		JOIN [development-competence-opal-at6qr].[org].[UserGroup] ug ON ugm.UserGroupID = ug.UserGroupID  AND ugm.EntityStatusID = 1 AND ug.EntityStatusID = 1
		JOIN [development-competence-opal-at6qr].[org].[User] uApprovalGroup ON ug.UserID = uApprovalGroup.UserID
		JOIN [development-competence-opal-at6qr].[org].[UserGroupType] ugt ON ug.UserGroupTypeID = ugt.UserGroupTypeID AND ugt.ExtId IN ('PrimaryAppovalGroup', 'AlternativeAppovalGroup')
		Join [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE uApprovalGroup.ExtID IS NOT NULL
	END
	
	BEGIN	
		UPDATE 
			[development-courses-opal-db].[dbo].[Users]
		SET
			[development-courses-opal-db].[dbo].[Users].PrimaryApprovingOfficerId = ISNULL(TempTbl.ExtID, '00000000-0000-0000-0000-000000000000')
		FROM
			[development-courses-opal-db].[dbo].[Users]
			LEFT JOIN #TempTable TempTbl
		ON [development-courses-opal-db].[dbo].[Users].Id = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'PrimaryAppovalGroup'
	END

	BEGIN
		UPDATE 
			[development-courses-opal-db].[dbo].[Users]
		SET
			[development-courses-opal-db].[dbo].[Users].AlternativeApprovingOfficerId = TempTbl.ExtID
		FROM
			[development-courses-opal-db].[dbo].[Users]
			LEFT JOIN #TempTable TempTbl
		ON [development-courses-opal-db].[dbo].[Users].Id = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'AlternativeAppovalGroup'
	END

	BEGIN
		UPDATE
			[development-courses-opal-db].[dbo].[Registration]
		SET
			[development-courses-opal-db].[dbo].[Registration].[ApprovingOfficer] = ISNULL(TempTbl.ExtID, '00000000-0000-0000-0000-000000000000')
		FROM
			[development-courses-opal-db].[dbo].[Registration]
			LEFT JOIN #TempTable TempTbl
		ON [development-courses-opal-db].[dbo].[Registration].[UserId] = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'PrimaryAppovalGroup'
		WHERE [development-courses-opal-db].[dbo].[Registration].[LearningStatus] IN ('NotStarted')
	END

	BEGIN
		UPDATE
			[development-courses-opal-db].[dbo].[Registration]
		SET
			[development-courses-opal-db].[dbo].[Registration].[AlternativeApprovingOfficer] = TempTbl.ExtID
		FROM
			[development-courses-opal-db].[dbo].[Registration]
			LEFT JOIN #TempTable TempTbl
		ON [development-courses-opal-db].[dbo].[Registration].[UserId] = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'AlternativeAppovalGroup'
		WHERE [development-courses-opal-db].[dbo].[Registration].[LearningStatus] IN ('NotStarted')
	END
END

IF (OBJECT_ID('tempdb..#TempTable') IS NOT NULL)
BEGIN
    DROP TABLE #TempTable
END
