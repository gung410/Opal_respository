-----UPDATE USERMETADATE CASE TYPE = 'teachingLevels'
IF (OBJECT_ID('tempdb.dbo.#Temp1') IS NOT NULL)
BEGIN
    DROP TABLE #Temp1
END

BEGIN
	BEGIN
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier, TempTable.ExtID) as [userId], TempTable.TeachingLevel as [value]
		INTO #Temp1
		FROM (Select T1.ExtID, T2.TeachingLevel TeachingLevel
				from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingLevels') TeachingLevels  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
				WHERE [DynamicAttributes] LIKE '%teachingLevels%' AND JSON_QUERY([DynamicAttributes],'$.teachingLevels') IS NOT NULL) T1
				cross apply OPENJSON(T1.TeachingLevels) with (TeachingLevel UNIQUEIDENTIFIER '$') as T2) TempTable
		Where NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingLevel)

	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'TeachingLevel', [value]
		FROM #Temp1

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET TeachingLevel = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp1
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp1') IS NOT NULL)
BEGIN
    DROP TABLE #Temp1
END

-----UPDATE USERMETADATE CASE TYPE = 'teachingSubjects'
IF (OBJECT_ID('tempdb.dbo.#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

BEGIN
	BEGIN	
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier,TempTable.ExtID) as [userId], TempTable.TeachingSubject as [value]
		INTO #Temp2
		FROM (Select T1.ExtID, T2.TeachingSubject TeachingSubject
				from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingSubjects') TeachingSubjects  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
				WHERE [DynamicAttributes] LIKE '%teachingSubjects%' AND JSON_QUERY([DynamicAttributes],'$.teachingSubjects') IS NOT NULL) T1
				cross apply OPENJSON(T1.TeachingSubjects) with (TeachingSubject UNIQUEIDENTIFIER '$') as T2) TempTable
		Where NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingSubject)

	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'TeachingSubject', [value]
		FROM #Temp2

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET TeachingSubject = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp2
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)

	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

-----UPDATE USERMETADATE CASE TYPE = 'teachingCourseOfStudy'
IF (OBJECT_ID('tempdb.dbo.#Temp3') IS NOT NULL)
BEGIN
    DROP TABLE #Temp3
END

BEGIN
	BEGIN	
		
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier,TempTable.ExtID) as [userId], TempTable.TeachingCourseOfStudy as [value]
		INTO #Temp3
		FROM (Select T1.ExtID, T2.TeachingCourseOfStudy TeachingCourseOfStudy
				from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingCourseOfStudy') TeachingCourseOfStudy  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
				WHERE [DynamicAttributes] LIKE '%teachingCourseOfStudy%' AND JSON_QUERY([DynamicAttributes],'$.teachingCourseOfStudy') IS NOT NULL) T1
				cross apply OPENJSON(T1.TeachingCourseOfStudy) with (TeachingCourseOfStudy UNIQUEIDENTIFIER '$') as T2) TempTable
		Where NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingCourseOfStudy)

	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'TeachingCourseOfStudy', [value]
		FROM #Temp3

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET TeachingCourseOfStudy = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp3
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)

	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp3') IS NOT NULL)
BEGIN
    DROP TABLE #Temp3
END

-----UPDATE USERMETADATE CASE TYPE = 'CocurricularActivity'
IF (OBJECT_ID('tempdb.dbo.#Temp4') IS NOT NULL)
BEGIN
    DROP TABLE #Temp4
END

BEGIN
	BEGIN	
			
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier,TempTable.ExtID) as [userId], TempTable.CocurricularActivity as [value]
		INTO #Temp4
		FROM (Select T1.ExtID, T2.CocurricularActivity CocurricularActivity
				from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.cocurricularActivities') CocurricularActivity  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
				WHERE [DynamicAttributes] LIKE '%cocurricularActivities%' AND JSON_QUERY([DynamicAttributes],'$.cocurricularActivities') IS NOT NULL) T1
				cross apply OPENJSON(T1.CocurricularActivity) with (CocurricularActivity UNIQUEIDENTIFIER '$') as T2) TempTable
		Where NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.CocurricularActivity)

	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'CocurricularActivity', [value]
		FROM #Temp4

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET CocurricularActivity = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp4
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)

	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp4') IS NOT NULL)
BEGIN
    DROP TABLE #Temp4
END

-----UPDATE USERMETADATE CASE TYPE = 'JobFamily'
IF (OBJECT_ID('tempdb.dbo.#Temp5') IS NOT NULL)
BEGIN
    DROP TABLE #Temp5
END

BEGIN
	BEGIN	
			
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier,TempTable.ExtID) as [userId], TempTable.JobFamily as [value]
		INTO #Temp5
		FROM (Select T1.ExtID, T2.JobFamily JobFamily
				from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.jobFamily') JobFamily  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
				WHERE [DynamicAttributes] LIKE '%jobFamily%' AND JSON_QUERY([DynamicAttributes],'$.jobFamily') IS NOT NULL) T1
				cross apply OPENJSON(T1.JobFamily) with (JobFamily UNIQUEIDENTIFIER '$') as T2) TempTable
		Where NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.JobFamily)

	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'JobFamily', [value]
		FROM #Temp5

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET JobFamily = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp5
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)

	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp5') IS NOT NULL)
BEGIN
    DROP TABLE #Temp5
END

-----UPDATE USERMETADATE CASE TYPE = 'Track'
IF (OBJECT_ID('tempdb.dbo.#Temp6') IS NOT NULL)
BEGIN
    DROP TABLE #Temp6
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp6
		from [development-competence-opal-at6qr].[org].[User] U
		JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
		JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 45
		JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId]
                                                                                                AND m.[Value] = UT.ExtID)
	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'Track', [value]
		FROM #Temp6

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET Track = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp6
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp6') IS NOT NULL)
BEGIN
    DROP TABLE #Temp6
END

-----UPDATE USERMETADATE CASE TYPE = 'DevelopmentalRole'
IF (OBJECT_ID('tempdb.dbo.#Temp7') IS NOT NULL)
BEGIN
    DROP TABLE #Temp7
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp7
		from [development-competence-opal-at6qr].[org].[User] U
		JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
		JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 49
		JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId]
                                                                                                    AND m.[Value] = UT.ExtID)
	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'DevelopmentalRole', [value]
		FROM #Temp7

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET DevelopmentalRole = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp7
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp7') IS NOT NULL)
BEGIN
    DROP TABLE #Temp7
END

-----UPDATE USERMETADATE CASE TYPE = 'EasSubstantiveGradeBanding'
IF (OBJECT_ID('tempdb.dbo.#Temp8') IS NOT NULL)
BEGIN
    DROP TABLE #Temp8
END

BEGIN
	BEGIN
		SELECT distinct userId as [userId], ExtID as [value]
		INTO #Temp8
		FROM (
			select cUser.Id as userId ,UT.ExtID
			from [development-competence-opal-at6qr].[org].[User] U
			JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
			JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 51 AND UT.ParentID=104
			JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
			WHERE NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId])
		) AS subquery
	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'EasSubstantiveGradeBanding', [value]
		FROM #Temp8

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET EasSubstantiveGradeBanding = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp8
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp8') IS NOT NULL)
BEGIN
    DROP TABLE #Temp8
END


-----UPDATE USER STATUS
IF (OBJECT_ID('tempdb.dbo.#Temp9') IS NOT NULL)
BEGIN
    DROP TABLE #Temp9
END

BEGIN 
	BEGIN
		SELECT distinct cUser.Id as [Id] ,E.CodeName AS [Status]
		INTO #Temp9
		from [development-competence-opal-at6qr].[org].[User] U
		JOIN [development-competence-opal-at6qr].[dbo].[EntityStatus] E ON U.EntityStatusID = E.EntityStatusID
		JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
	END

	BEGIN	
		UPDATE 
			[development-courses-opal-db].[dbo].[Users]
		SET
			[development-courses-opal-db].[dbo].[Users].[Status] = TempTbl.[Status]
		FROM
			[development-courses-opal-db].[dbo].[Users]
			INNER JOIN #Temp9 TempTbl
		ON [development-courses-opal-db].[dbo].[Users].Id = TempTbl.Id
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp9') IS NOT NULL)
BEGIN
    DROP TABLE #Temp9
END

----------------------------- Migrate Designation from SAM
IF (OBJECT_ID('tempdb.dbo.#Temp10') IS NOT NULL)
BEGIN
    DROP TABLE #Temp10
END

BEGIN
	BEGIN
		SELECT DISTINCT TRY_CONVERT(uniqueidentifier,TempTable.ExtID)  as [userId], TempTable.Designation as [value]
		INTO #Temp10
		FROM ( SELECT [ExtID], JSON_VALUE([DynamicAttributes],'$.designation') Designation  
				FROM [development-competence-opal-at6qr].[org].[User] sUser
				JOIN [development-courses-opal-db].[dbo].[Users] cUser ON CONVERT(nvarchar(256),cUser.Id) = sUser.ExtID
				WHERE [DynamicAttributes] LIKE '%designation%' AND JSON_VALUE([DynamicAttributes],'$.designation') IS NOT NULL) TempTable
		Where NOT EXISTS (SELECT * FROM [development-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(nvarchar(256),uTemp.UserId) = TempTable.ExtID AND 
																											uTemp.[Value] = TempTable.Designation)
	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'Designation', [value]
		FROM #Temp10

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET Designation = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp10
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp10') IS NOT NULL)
BEGIN
    DROP TABLE #Temp10
END
------------------------------ Migrate LearningFramework from SAM
IF (OBJECT_ID('tempdb.dbo.#Temp11') IS NOT NULL)
BEGIN
    DROP TABLE #Temp11
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp11
		from [development-competence-opal-at6qr].[org].[User] U 
		JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID 
		JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 51 
		JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE NOT EXISTS (SELECT * FROM [development-courses-opal-db].[dbo].[UserMetadata] m
								WHERE cUser.Id = m.[UserId] AND m.[Value] = UT.ExtID) 
	END
	BEGIN	
		INSERT INTO [development-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'LearningFramework', [value]
		FROM #Temp11

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET LearningFramework = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp11
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
	END
END

IF (OBJECT_ID('tempdb.dbo.#Temp11') IS NOT NULL)
BEGIN
    DROP TABLE #Temp11
END

--- Migrate UserSystemRole
IF (OBJECT_ID('tempdb.dbo.#Temp12') IS NOT NULL)
BEGIN
    DROP TABLE #Temp12
END

BEGIN
	BEGIN
    BEGIN
        select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
        INTO #Temp12
        from [development-competence-opal-at6qr].[org].[User] U
        JOIN [development-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
        JOIN [development-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 43
        JOIN [development-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
        WHERE NOT EXISTS (SELECT 1 FROM [development-courses-opal-db].[dbo].[UserSystemRole] m WHERE cUser.Id=m.[UserId] AND [value] = m.[Value])
    END
    BEGIN
        UPDATE #Temp12
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
        FROM #Temp12

		UPDATE [development-courses-opal-db].[dbo].[Users]
		SET SystemRoles = (SELECT '['+
								SUBSTRING(
									(
										SELECT ',"' + CONVERT(nvarchar(100), [value]) +'"'
										FROM #Temp12
										WHERE [userId] = [Id] AND [value] IS NOT NULL 
										FOR XML PATH ('')
									), 2, 1000) + ']'
							)
    END
END
END

IF (OBJECT_ID('tempdb.dbo.#Temp12') IS NOT NULL)
BEGIN
    DROP TABLE #Temp12
END


--Migrate FullTextColumn
-- Class Run
UPDATE [development-courses-opal-db].[dbo].[ClassRun]
SET FacilitatorIdsFullTextSearch = case when FacilitatorIds = '[]' then NULL else FacilitatorIds end

UPDATE [development-courses-opal-db].[dbo].[ClassRun]
SET CoFacilitatorIdsFullTextSearch = case when CoFacilitatorIds = '[]' then NULL else CoFacilitatorIds end

-- Course
UPDATE [development-courses-opal-db].[dbo].[Course]
SET CourseFacilitatorIdsFullTextSearch = case when CourseFacilitatorIds = '[]' then NULL else CourseFacilitatorIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET CourseCoFacilitatorIdsFullTextSearch = case when CourseCoFacilitatorIds = '[]' then NULL else CourseCoFacilitatorIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET CollaborativeContentCreatorIdsFullTextSearch = case when CollaborativeContentCreatorIds = '[]' then NULL else CollaborativeContentCreatorIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET DevelopmentalRoleIdsFullTextSearch = case when DevelopmentalRoleIds = '[]' then NULL else DevelopmentalRoleIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET ServiceSchemeIdsFullTextSearch = case when ServiceSchemeIds = '[]' then NULL else ServiceSchemeIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET SubjectAreaIdsFullTextSearch = case when SubjectAreaIds = '[]' then NULL else SubjectAreaIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningFrameworkIdsFullTextSearch = case when LearningFrameworkIds = '[]' then NULL else LearningFrameworkIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningDimensionIdsFullTextSearch = case when LearningDimensionIds = '[]' then NULL else LearningDimensionIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningAreaIdsFullTextSearch = case when LearningAreaIds = '[]' then NULL else LearningAreaIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningAreaIdsFullTextSearch = case when LearningAreaIds = '[]' then NULL else LearningAreaIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningAreaIdsFullTextSearch = case when LearningAreaIds = '[]' then NULL else LearningAreaIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET LearningSubAreaIdsFullTextSearch = case when LearningSubAreaIds = '[]' then NULL else LearningSubAreaIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET CategoryIdsFullTextSearch = case when CategoryIds = '[]' then NULL else CategoryIds end

UPDATE [development-courses-opal-db].[dbo].[Course]
SET TeachingLevelsFullTextSearch = case when TeachingLevels = '[]' then NULL else TeachingLevels end

-- Users
UPDATE [development-courses-opal-db].[dbo].[Users]
SET ServiceSchemeFullTextSearch = case when ServiceScheme = '[]' then NULL else ServiceScheme end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET TeachingCourseOfStudyFullTextSearch = case when TeachingCourseOfStudy = '[]' then NULL else TeachingCourseOfStudy end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET DesignationFullTextSearch = case when Designation = '[]' then NULL else Designation end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET DevelopmentalRoleFullTextSearch = case when DevelopmentalRole = '[]' then NULL else DevelopmentalRole end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET TeachingLevelFullTextSearch = case when TeachingLevel = '[]' then NULL else TeachingLevel end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET TeachingSubjectFullTextSearch = case when TeachingSubject = '[]' then NULL else TeachingSubject end

UPDATE [development-courses-opal-db].[dbo].[Users]
SET LearningFrameworkFullTextSearch = case when LearningFramework = '[]' then NULL else LearningFramework end
