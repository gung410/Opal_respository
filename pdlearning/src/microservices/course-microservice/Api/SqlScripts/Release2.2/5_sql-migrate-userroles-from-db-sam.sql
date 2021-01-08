IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

-----UPDATE USERSYSTEMROLE
IF (OBJECT_ID('tempdb..#Temp4') IS NOT NULL)
BEGIN
    DROP TABLE #Temp4
END

BEGIN
	BEGIN
    BEGIN
        select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
        INTO #Temp4
        from [development-competence-opal-at6qr].[org].[User] U
        JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
        JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 43
        JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
        --WHERE NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserSystemRole] m WHERE cUser.Id=m.[UserId] AND [value] = m.[Value])
    END
    BEGIN
        UPDATE #Temp4
        SET [Value] =
        ( CASE 
        WHEN ([value] = 'divisionadmin') THEN 'Division Administrator'
        WHEN ([value] = 'branchadmin') THEN 'Branch Administrator'
        WHEN ([value] = 'schooladmin') THEN 'School Administrator'
        WHEN ([value] = 'approvingofficer') THEN 'OPJ Approving Officer'
        WHEN ([value] = 'reportingofficer') THEN 'Course Approving Officer'
        WHEN ([value] = 'learner') THEN 'Learner'
        WHEN ([value] = 'overallsystemadministrator') THEN 'System Administrator'
        WHEN ([value] = 'schoolcontentapprovingofficer') THEN 'School Content Approving Officer'
        WHEN ([value] = 'MOEHQcontentapprovingofficer') THEN 'MOEHQ Content Approving Officer'
        WHEN ([value] = 'webpageeditor') THEN 'Web Page Editor'
        WHEN ([value] = 'courseplanningcoordinator') THEN 'Course Planning Coordinator'
        WHEN ([value] = 'coursecontentcreator') THEN 'Course Content Creator'
        WHEN ([value] = 'courseadmin') THEN 'Course Administrator'
        WHEN ([value] = 'coursefacilitator') THEN 'Course Facilitator'
        WHEN ([value] = 'contentcreator') THEN 'Content Creator'
        WHEN ([value] = 'schooltrainingcoordinator') THEN 'School Staff Developer'
        WHEN ([value] = 'useraccountadministrator') THEN 'User Account Administrator'
        WHEN ([value] = 'divisiontrainingcoordinator') THEN 'Divisional Training Coordinator'
        ELSE ([value])
        END)
        WHERE [value] IN ('divisionadmin','branchadmin','schooladmin','approvingofficer',
                            'reportingofficer','learner','overallsystemadministrator','schoolcontentapprovingofficer',
                            'MOEHQcontentapprovingofficer','webpageeditor','courseplanningcoordinator','coursecontentcreator',
                            'courseadmin','coursefacilitator','contentcreator','schooltrainingcoordinator','useraccountadministrator',
                            'divisiontrainingcoordinator')
    END
    BEGIN    
        INSERT INTO [development-courses-opal-db].[dbo].[UserSystemRole] (Id, CreatedDate, UserId, [Value])
        SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), [value]
        FROM #Temp4
    END
END
END

IF (OBJECT_ID('tempdb..#Temp4') IS NOT NULL)
BEGIN
    DROP TABLE #Temp4
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO
SET NOEXEC OFF
GO
