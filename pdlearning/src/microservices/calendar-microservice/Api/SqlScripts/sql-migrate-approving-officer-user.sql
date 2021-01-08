IF (OBJECT_ID('tempdb..#TempTableCalendar') IS NOT NULL)
BEGIN
    DROP TABLE #TempTableCalendar
END
GO

BEGIN
	BEGIN
		SELECT DISTINCT cUser.Id, uApprovalGroup.ExtID, ugt.ExtId as UserGroupTypeExtId
		INTO #TempTableCalendar
		FROM [development-competence-opal-at6qr].[org].[User] u
		JOIN [development-competence-opal-at6qr].[org].[UGMember] ugm ON u.UserID = ugm.UserId AND ugm.EntityStatusID = 1
		JOIN [development-competence-opal-at6qr].[org].[UserGroup] ug ON ugm.UserGroupID = ug.UserGroupID  AND ugm.EntityStatusID = 1 AND ug.EntityStatusID = 1
		JOIN [development-competence-opal-at6qr].[org].[User] uApprovalGroup ON ug.UserID = uApprovalGroup.UserID
		JOIN [development-competence-opal-at6qr].[org].[UserGroupType] ugt ON ug.UserGroupTypeID = ugt.UserGroupTypeID AND ugt.ExtId IN ('PrimaryAppovalGroup', 'AlternativeAppovalGroup')
		JOIN [development-calendar-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE uApprovalGroup.ExtID IS NOT NULL
	END
	
	BEGIN	
		UPDATE 
			[development-calendar-opal-db].[dbo].[Users]
		SET
			[development-calendar-opal-db].[dbo].[Users].PrimaryApprovalOfficerId = ISNULL(TempTbl.ExtID, NULL)
		FROM
			[development-calendar-opal-db].[dbo].[Users]
			LEFT JOIN #TempTableCalendar TempTbl
		ON [development-calendar-opal-db].[dbo].[Users].Id = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'PrimaryAppovalGroup'
	END

	BEGIN
		UPDATE 
			[development-calendar-opal-db].[dbo].[Users]
		SET
			[development-calendar-opal-db].[dbo].[Users].AlternativeApprovalOfficerId = ISNULL(TempTbl.ExtID, NULL)
		FROM
			[development-calendar-opal-db].[dbo].[Users]
			LEFT JOIN #TempTableCalendar TempTbl
		ON [development-calendar-opal-db].[dbo].[Users].Id = TempTbl.Id AND TempTbl.UserGroupTypeExtId = 'AlternativeAppovalGroup'
	END
END

IF (OBJECT_ID('tempdb..#TempTableCalendar') IS NOT NULL)
BEGIN
    DROP TABLE #TempTableCalendar
END
